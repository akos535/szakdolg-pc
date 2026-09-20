using System;
using System.Collections.Generic;
using System.Text;

using Windows.Media.Control;

namespace Remote.Client;

internal class ClientProgram
{
    private GlobalSystemMediaTransportControlsSession _session;
    public static int Main(string[] args)
    {
        Console.WriteLine(_session.GetTimelineProperties());
        return 0;
    }

}
