using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Remote.Core.Command_Handlers;
using Remote.Core.Media;

namespace Remote.Server;

internal sealed class RemoteServer : IAsyncDisposable
{
    private WebApplication? _app;
    private readonly ICommandHandler _commandHandler;
    private readonly MediaInfoProvider _mediaInfoProvider;
    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public event EventHandler<string>? StatusChanged;

    public RemoteServer(ICommandHandler commandHandler, MediaInfoProvider mediaInfoProvider)
    {
        _commandHandler = commandHandler;
        _mediaInfoProvider = mediaInfoProvider;
    }

    public async Task StartAsync()
    {
        if (_app is not null)
            return;

        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        builder.WebHost.UseUrls("http://0.0.0.0:5000");

        WebApplication app = builder.Build();

        app.UseWebSockets();

        app.Map("/ws", async context =>
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                context.Response.StatusCode = 400;
                return;
            }

            using WebSocket socket = await context.WebSockets.AcceptWebSocketAsync();
            using CancellationTokenSource connectionCancellation = CancellationTokenSource.CreateLinkedTokenSource(context.RequestAborted);

            ReportStatus("Phone connected");
            
            try
            {
                Task receiveTask = RecieveMessagesAsync(socket, connectionCancellation.Token);
                Task mediaTask = SendMediaInfoLoopAsync(socket, connectionCancellation.Token);

                await receiveTask;
                connectionCancellation.Cancel();
                await mediaTask;
            }
            catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested){}
            catch(WebSocketException ex)
            {
                ReportStatus($"Connection lost : {ex.Message}");
            }
            finally
            {
                ReportStatus("Phone disconnected");
            }
        });

        try
        {
            await app.StartAsync();
            _app = app;

            ReportStatus("Waiting for phone");
        }
        catch
        {
            await app.DisposeAsync();
            throw;
        }
    }

    private async Task RecieveMessagesAsync(WebSocket socket, CancellationToken cancellationToken)
    {
        byte[] buffer = new byte[4096];

        while (socket.State == WebSocketState.Open)
        {
            using MemoryStream message = new MemoryStream();
            WebSocketReceiveResult result;

            do
            {
                result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "connection closed", cancellationToken);
                    return;
                }

                if (result.MessageType != WebSocketMessageType.Text)
                {
                    await socket.CloseAsync(WebSocketCloseStatus.InvalidMessageType, "not json", cancellationToken);
                }

                message.Write(buffer, 0, result.Count);
            }
            while (!result.EndOfMessage);

            byte[] bytes = message.ToArray();
            string json = Encoding.UTF8.GetString(bytes);

            Console.WriteLine($"Recieved: {json}");

            try
            {
                await _commandHandler.HandleAsync(json, cancellationToken);
            }
            catch (JsonException ex)
            {

                Console.WriteLine($"Invalid JSON: {ex.Message}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Invalid command: {ex.Message}");
            }
        }
    }

    private async Task SendMediaInfoLoopAsync(WebSocket socket, CancellationToken cancellationToken)
    {
        string? previousMediaJson = null;

        try
        {
            while (socket.State == WebSocketState.Open)
            {
                MediaInfo? mediaInfo = await _mediaInfoProvider.GetMediaPropertiesAsync();

                byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(
                    new
                    {
                        type = "media.info",
                        payload = mediaInfo
                    }, _options);

                string currentMediaJson = Encoding.UTF8.GetString(bytes);

                if (currentMediaJson != previousMediaJson)
                {
                    await socket.SendAsync(
                        new ArraySegment<byte>(bytes),
                        WebSocketMessageType.Text,
                        endOfMessage: true,
                        cancellationToken: cancellationToken);

                    previousMediaJson = currentMediaJson;
                }

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) {}
        catch (WebSocketException) {}
    }

    private void ReportStatus(string message)
    {
        StatusChanged?.Invoke(this, message);
    }

    public async ValueTask DisposeAsync()
    {
        if (_app is null)
            return;

        try
        {
            await _app.StopAsync();
        }
        finally
        {
            await _app.DisposeAsync();
            _app = null;
        }
    }
}