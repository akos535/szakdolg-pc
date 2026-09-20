using Windows.Media.Control;

using static System.Collections.Specialized.BitVector32;

namespace Remote.Core;

internal class MediaProperties
{
    internal static Dictionary<string, string> GetMediaPropertiesAsync()
    {
        return Task.Run(() => GetMediaProperties()).GetAwaiter().GetResult();
    }
    internal static async Task<Dictionary<string, string>> GetMediaProperties()
    {
        GlobalSystemMediaTransportControlsSession? session = await GetSessionAsync();


        if (session is null)
        {
            Console.WriteLine("Suck my fucking balls.");
            return [];
        }

        Console.WriteLine($"App: {session.SourceAppUserModelId}");
        GlobalSystemMediaTransportControlsSessionMediaProperties media =
            await session.TryGetMediaPropertiesAsync();

        Dictionary<string, string> info = new Dictionary<string, string>()
        {
            {"TITLE", media.Title},
            {"ARTIST", media.Artist},
            {"ALBUM", media.AlbumTitle}
        };

        return info;
    }
    private static async Task<GlobalSystemMediaTransportControlsSession> GetSessionAsync()
    {
        GlobalSystemMediaTransportControlsSessionManager sessionManager =
            await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();

        return sessionManager.GetCurrentSession();
    }
}
