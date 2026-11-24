using System.Collections.Concurrent;

using Remote.Core.Command_Handlers;
using Remote.Core.Commands;
using Remote.Core.Media;

namespace Remote.Server;

public sealed class ServerRunner : IAsyncDisposable
{
    private RemoteServer? _remoteServer;
    private UdpServer? _udpServer;

    private readonly ConcurrentDictionary<string, byte> _approvedRequesters = new();
    private readonly ConcurrentDictionary<string, Lazy<Task<bool>>> _pendingApprovals = new();
    public bool IsRunning => _remoteServer != null;

    public event EventHandler<string>? StatusChanged;
    public event Func<string, Task<bool>>? ConnectionRequested;

    private void RemoteServer_StatusChanged(object? sender, string message)
    {
        StatusChanged?.Invoke(this, message);
    }

    private async Task<bool> RequestConnectionApprovalAsync(string requester)
    {
        if (_approvedRequesters.ContainsKey(requester)) return true;

        Lazy<Task<bool>> pendingApproval = _pendingApprovals.GetOrAdd(
            requester,
            key => new Lazy<Task<bool>>(() => AskForApprovalAsync(key)));

        try
        {
            return await pendingApproval.Value;
        } 
        finally
        {
            _pendingApprovals.TryRemove(requester, out _);
        }
    }

    private async Task<bool> AskForApprovalAsync(string requester)
    {
        Func<string, Task<bool>>? connectionRequested = ConnectionRequested;

        if (connectionRequested == null) return false;

        bool approved = await connectionRequested(requester);

        if (approved) _approvedRequesters.TryAdd(requester, 0);

        return approved;
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

        RemoteServer remoteServer = new(handler, mediaInfoProvider)
        {
            ConnectionApproval = RequestConnectionApprovalAsync
        };
        remoteServer.StatusChanged += RemoteServer_StatusChanged;

        try
        {
            await remoteServer.StartAsync();

            UdpServer udpServer = new(handler, remoteServer.Port)
            {
                ConnectionApproval = RequestConnectionApprovalAsync
            };
            await udpServer.StartAsync();

            _remoteServer = remoteServer;
            _udpServer = udpServer;

            StatusChanged?.Invoke(this, $"Server started. Waiting for phone.");
        }
        catch
        {
            remoteServer.StatusChanged -= RemoteServer_StatusChanged;
            await remoteServer.DisposeAsync();
            throw;
        }
    }

    public async ValueTask DisposeAsync()
    {
        RemoteServer? remoteServer = _remoteServer;
        UdpServer? udpServer = _udpServer;

        _remoteServer = null;
        _udpServer = null;

        if(remoteServer != null)
        {
            remoteServer.StatusChanged -= RemoteServer_StatusChanged;
            await remoteServer.DisposeAsync();
        }
        if (udpServer != null)
        {
            await udpServer.DisposeAsync();
        }

        _approvedRequesters.Clear();
        _pendingApprovals.Clear();
    }
}