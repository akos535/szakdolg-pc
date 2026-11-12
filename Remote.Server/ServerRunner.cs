using Remote.Core.Command_Handlers;
using Remote.Core.Commands;
using Remote.Core.Media;

namespace Remote.Server;

public sealed class ServerRunner : IAsyncDisposable
{
    private RemoteServer _remoteServer;
    private UdpServer _udpServer;
    public bool IsRunning => _remoteServer != null;

    public event EventHandler<string>? StatusChanged;

    private void RemoteServer_StatusChanged(object? sender, string message)
    {
        StatusChanged?.Invoke(this, message);
    }

    public async Task StartAsync()
    {
        if (IsRunning) return;

        MediaSessionProvider sessionProvider = await MediaSessionProvider.CreateAsync();
        
        MediaInfoProvider mediaInfoProvider = new(sessionProvider);

        ICommand[] commands = [
                new KeyPressCommand(),
                new KeyDownCommand(),
                new KeyUpCommand(),
                new MouseMoveCommand(),
                new MouseClickCommand(),
                new DelayCommand()
            ];

        ICommand macroCommand = new MacroCommand(commands);
        CommandHandler handler = new(commands.Append(macroCommand));

        RemoteServer remoteServer = new(handler, mediaInfoProvider);
        remoteServer.StatusChanged += RemoteServer_StatusChanged;
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
            _remoteServer.StatusChanged -= RemoteServer_StatusChanged;
            await _remoteServer.DisposeAsync();
        }
        if (_udpServer != null)
        {
            await _udpServer.DisposeAsync();
        }
    }
}