using Remote.Core.Command_Handlers;
using Remote.Core.Commands;
using Remote.Core.Media;

namespace Remote.Server;

public sealed class ServerRunner : IAsyncDisposable
{
    private RemoteServer _remoteServer;
    private UdpServer _udpServer;
    public bool IsRunning => _remoteServer != null;

    public async Task StartAsync()
    {
        if (IsRunning) return;

        MediaSessionProvider sessionProvider = MediaSessionProvider.CreateAsync().GetAwaiter().GetResult();
        
        MediaInfoProvider mediaInfoProvider = new(sessionProvider);

        CommandHandler handler = new CommandHandler(
            new ICommand[]
            {
                new KeyPressCommand(),
                new MouseMoveCommand()
            });


        RemoteServer remoteServer = new(handler, mediaInfoProvider);
        UdpServer udpServer = new(handler);

        try
        {
            await remoteServer.StartAsync();
            await udpServer.StartAsync();

            _remoteServer = remoteServer;
            _udpServer = udpServer;
        }
        catch
        {
            await remoteServer.DisposeAsync();
            await udpServer.DisposeAsync();
            throw;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if(_remoteServer != null)
        {
            await _remoteServer.DisposeAsync();
        }
        if (_udpServer != null)
        {
            await _udpServer.DisposeAsync();
        }
    }
}