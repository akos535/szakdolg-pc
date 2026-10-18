using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Remote.Core.Command_Handlers;

namespace Remote.Server;

public sealed class RemoteServer : IAsyncDisposable
{
    private WebApplication? _app;
    private readonly ICommandHandler _commandHandler;

    public RemoteServer(ICommandHandler commandHandler)
    {
        _commandHandler = commandHandler;
    }

    public async Task StartAsync()
    {
        if (_app is not null)
            return;

        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseUrls("http://0.0.0.0:5000");

        var app = builder.Build();

        app.UseWebSockets();

        app.Map("/ws", async context =>
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                context.Response.StatusCode = 400;
                return;
            }

            using var socket = await context.WebSockets.AcceptWebSocketAsync();

            Console.WriteLine("Phone connected.");
            
            try
            {
                await RecieveMessagesAsync(socket, context.RequestAborted);
            }
            catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
            {
                Console.WriteLine("connection cancelled.");
            }
            catch(WebSocketException ex)
            {
                Console.WriteLine($"connection lost : {ex.Message}");
            }
        });

        try
        {
            await app.StartAsync();
            _app = app;

            Console.WriteLine("Server listening on port 5000.");
        }
        catch
        {
            await app.DisposeAsync();
            throw;
        }
    }
    private async Task RecieveMessagesAsync(WebSocket socket, CancellationToken cancellationToken)
    {
        var buffer = new byte[4096];

        while (socket.State == WebSocketState.Open)
        {
            using var message = new MemoryStream();
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

            var bytes = message.ToArray();
            var json = Encoding.UTF8.GetString(bytes);

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