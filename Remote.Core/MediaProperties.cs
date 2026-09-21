using Windows.Media.Control;

namespace Remote.Core;

internal class MediaProperties : SessionFetch
{
    internal static MediaInfo? GetMediaProperties()
    {
        return Task.Run(() => GetMediaPropertiesAsync()).GetAwaiter().GetResult();
    }
    internal static async Task<MediaInfo?> GetMediaPropertiesAsync()
    {
        GlobalSystemMediaTransportControlsSession? session = await SessionFetch.GetSessionAsync();

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
