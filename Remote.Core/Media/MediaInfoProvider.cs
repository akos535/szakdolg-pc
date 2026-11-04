using Windows.Media.Control;
using Windows.Storage.Streams;

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


        byte[]? thumbnail = null;
        string? thumbnailType = null;

        if (media.Thumbnail != null)
        {
            try
            {
                using IRandomAccessStreamWithContentType stream = await media.Thumbnail.OpenReadAsync();

                if (stream.Size > 0 /*&& stream.Size <= 5 * 1024 * 1024*/)
                {
                    using DataReader reader = new(stream.GetInputStreamAt(0));

                    uint loaded = await reader.LoadAsync((uint)stream.Size);

                    if( (ulong)loaded == stream.Size)
                    {
                        thumbnail = new byte[(int)loaded];
                        reader.ReadBytes(thumbnail);

                        thumbnailType = stream.ContentType;
                    }
                }
            } 
            catch (Exception ex)
            {
                Console.WriteLine($"No thumbnail found: {ex.Message}");
            }
        }

        return new(
            media.Title,
            media.Artist,
            media.AlbumTitle,
            session.SourceAppUserModelId,
            thumbnail,
            thumbnailType
        );
    }
}
