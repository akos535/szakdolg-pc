using System.Text;

using Windows.Media.Control;
using Windows.UI.Core;
using Remote.Core;

namespace Remote.Client;


internal class ClientProgram
{
    public static int Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        MediaInfo? info = MediaProperties.GetMediaProperties();

        if (info is not null)
        {
            Console.WriteLine($"App: {info.SourceApp}");
            Console.WriteLine($"Title: {info.Title}");
            Console.WriteLine($"Artist: {info.Artist}");
            Console.WriteLine($"Album: {info.Album}");
        }


        return 0;
    }
}
