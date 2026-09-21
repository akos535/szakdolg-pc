using System.Text;

using Windows.Media.Control;
using Windows.UI.Core;
using Windows.Devices.Printers;

using Remote.Core.Media;

namespace Remote.Client;


internal class ClientProgram
{
    public static int Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        GlobalSystemMediaTransportControlsSession session = SessionFetch.GetSession();

        MediaInfo? info = MediaProperties.GetMediaProperties(session);

        if (info is not null)
        {
            Console.WriteLine($"App: {info.SourceApp}");
            Console.WriteLine($"Title: {info.Title}");
            Console.WriteLine($"Artist: {info.Artist}");
            Console.WriteLine($"Album: {info.Album}");
        }

        Console.WriteLine(MediaController.PlayPause(session));

        return 0;
    }
}
