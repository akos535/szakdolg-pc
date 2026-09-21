using Windows.Media.Control;

namespace Remote.Core.Media;

internal class MediaProperties
{
    internal static MediaInfo? GetMediaProperties(GlobalSystemMediaTransportControlsSession session)
    {
        return GetMediaPropertiesAsync(session).GetAwaiter().GetResult();
    }
    internal static async Task<MediaInfo?> GetMediaPropertiesAsync(GlobalSystemMediaTransportControlsSession session)
    {
        if (session is null) return null;

        GlobalSystemMediaTransportControlsSessionMediaProperties media =
            await session.TryGetMediaPropertiesAsync();

        MediaInfo info = new(
            media.Title,
            media.Artist,
            media.AlbumTitle,
            session.SourceAppUserModelId
        );

        return info;
    }
}
