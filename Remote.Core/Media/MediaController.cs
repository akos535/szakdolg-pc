using Windows.Media.Control;
namespace Remote.Core.Media;

internal class MediaController
{
    public readonly MediaSessionProvider _sessionProvider;

    internal MediaController(MediaSessionProvider sessionProvider)
    {
        _sessionProvider = sessionProvider;
    }

    internal async Task<bool> PlayPauseAsync()
    {
        GlobalSystemMediaTransportControlsSession? session =
            _sessionProvider.GetCurrentSession();

        if (session is null) return false;

        return await session.TryTogglePlayPauseAsync();
    }

    internal async Task<bool> SkipNextAsync()
    {
        GlobalSystemMediaTransportControlsSession? session =
            _sessionProvider.GetCurrentSession();

        if (session is null) return false;

        return await session.TrySkipNextAsync();
    }

    internal async Task<bool> SkipPreviousAsync()
    {
        GlobalSystemMediaTransportControlsSession? session =
            _sessionProvider.GetCurrentSession();

        if (session is null) return false;

        return await session.TrySkipPreviousAsync();
    }
}
