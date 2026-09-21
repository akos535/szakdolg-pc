using Windows.Media.Control;

namespace Remote.Core;

internal class SessionFetch
{
    internal static async Task<GlobalSystemMediaTransportControlsSession> GetSessionAsync()
    {
        GlobalSystemMediaTransportControlsSessionManager sessionManager =
            await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();

        return sessionManager.GetCurrentSession();
    }
}