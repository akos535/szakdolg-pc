using System;
using System.Collections.Generic;
using System.Text;

using Windows.Media.Control;

namespace Remote.Core.Media;

internal class MediaSessionProvider
{
    private readonly GlobalSystemMediaTransportControlsSessionManager _sessionManager;

    private MediaSessionProvider(GlobalSystemMediaTransportControlsSessionManager sessionManager)
    {
        _sessionManager = sessionManager;
    }

    internal static async Task<MediaSessionProvider> CreateAsync()
    {
        GlobalSystemMediaTransportControlsSessionManager manager =
            await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();

        return new MediaSessionProvider(manager);
    }

    internal GlobalSystemMediaTransportControlsSession? GetCurrentSession()
    {
        return _sessionManager.GetCurrentSession();
    }
}
