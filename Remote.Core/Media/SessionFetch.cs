using Windows.Media.Control;

namespace Remote.Core.Media;

internal static class SessionFetch
{
    internal static GlobalSystemMediaTransportControlsSession GetSession()
    {
        return GetSessionAsync().GetAwaiter().GetResult();
    }
    internal static async Task<GlobalSystemMediaTransportControlsSession> GetSessionAsync()
    {
        GlobalSystemMediaTransportControlsSessionManager sessionManager =
            await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();

        return sessionManager.GetCurrentSession();
    }
}
