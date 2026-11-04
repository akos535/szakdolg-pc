using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

using Remote.Core.Command_Handlers;

namespace Remote.Server;

internal class UdpServer : IAsyncDisposable
{
    private UdpClient? _udpClient;
    private CancellationTokenSource? _cancellationTokenSource;
    private Task? _recieveTask;

    private readonly ICommandHandler _commandHandler;

    public UdpServer(ICommandHandler commandHandler)
    {
        _commandHandler = commandHandler;
    }

    public Task StartAsync()
    {
        if(_udpClient != null)
        {
            return Task.CompletedTask;
        }

        UdpClient udpClient = new(new IPEndPoint(IPAddress.Any, 5001));
        CancellationTokenSource cancellationTokenSource = new();
        _udpClient = udpClient;
        _cancellationTokenSource = cancellationTokenSource;

        _recieveTask = RecieveMessagesAsync(_udpClient, _cancellationTokenSource.Token);

        Console.WriteLine("UDP server listening on port 5001.");
        return Task.CompletedTask;
    }

    private async Task RecieveMessagesAsync(UdpClient udpClient, CancellationToken cancellationToken)
    {
        try
        {
            while(!cancellationToken.IsCancellationRequested)
            {
                UdpReceiveResult result = await udpClient.ReceiveAsync(cancellationToken);

                string json = Encoding.UTF8.GetString(result.Buffer);

                try
                {
                    using JsonDocument document = JsonDocument.Parse(json);
                    JsonElement root = document.RootElement;
                    if(root.ValueKind != JsonValueKind.Object || !root.TryGetProperty("type", out var typeElement) || typeElement.ValueKind != JsonValueKind.String)
                    {
                        continue;
                    }

                    var type = typeElement.GetString();

                    if(type == "discovery.request")
                    {
                        if(!root.TryGetProperty("requestId", out var requestIdElement) || requestIdElement.ValueKind != JsonValueKind.String)
                        {
                            continue;
                        }

                        var requestId = requestIdElement.GetString();

                        if (string.IsNullOrWhiteSpace(requestId)) continue;

                        byte[] response = JsonSerializer.SerializeToUtf8Bytes(new
                        {
                            type = "discovery.response",
                            requestId = requestId,
                            name = Environment.MachineName,
                            webSocketPort = 5000,
                            udpPort = 5001
                        });

                        await udpClient.SendAsync(response, result.RemoteEndPoint, cancellationToken);
                    }
                    else
                    {
                        await _commandHandler.HandleAsync(json, cancellationToken);
                    }

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
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { }
        catch (ObjectDisposedException) when (cancellationToken.IsCancellationRequested) { }
        catch(SocketException) when (cancellationToken.IsCancellationRequested) { }
        catch (Exception ex)
        {
            Console.WriteLine($"UDP loop stopped {ex.Message}");
            throw;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_udpClient == null)
        {
            return;
        }

        try
        {
            _cancellationTokenSource!.Cancel();
            _udpClient.Dispose();

            if(_recieveTask != null)
            {
                await _recieveTask;
            }
        }
        finally
        {
            _cancellationTokenSource!.Dispose();

            _udpClient = null;
            _cancellationTokenSource = null;
            _recieveTask = null;
        }
    }
}
