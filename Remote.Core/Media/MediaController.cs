using Windows.Media.Control;
namespace Remote.Core.Media;

internal static class MediaController
{
    internal static bool PlayPause(GlobalSystemMediaTransportControlsSession session)
    {
        return PlayPauseAsync(session).GetAwaiter().GetResult();
    }
    internal static async Task<bool> PlayPauseAsync(GlobalSystemMediaTransportControlsSession session)
    {

        if(session is null)
        {
            Console.WriteLine("No session");
            return false;
        }

        return await session.TryTogglePlayPauseAsync();
    }
}
