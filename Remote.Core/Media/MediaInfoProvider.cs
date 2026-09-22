using Windows.Media.Control;

namespace Remote.Core.Media;

internal class MediaInfoProvider
{
    private readonly MediaSessionProvider _sessionProvider;
    internal MediaInfoProvider(MediaSessionProvider sessionProvider)
    {
        _sessionProvider = sessionProvider;
    }

    internal async Task<MediaInfo?> GetMediaPropertiesAsync()
    {
        GlobalSystemMediaTransportControlsSession? session =
            _sessionProvider.GetCurrentSession();

        if (session is null) return null;

        GlobalSystemMediaTransportControlsSessionMediaProperties media =
            await session.TryGetMediaPropertiesAsync();

        return new(
            media.Title,
            media.Artist,
            media.AlbumTitle,
            session.SourceAppUserModelId
        ); ;
    }
}
