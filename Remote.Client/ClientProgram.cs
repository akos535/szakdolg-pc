using System.Text;
using Remote.Core.Media;

namespace Remote.Client;


internal class ClientProgram
{
    public static int Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        
        MediaSessionProvider sessionProvider = MediaSessionProvider.CreateAsync().GetAwaiter().GetResult();

        MediaController mediaController = new(sessionProvider);
        MediaInfoProvider mediaInfoProvider = new(sessionProvider);

        bool done = false;

        while (!done)
        {
            string? comm = Console.ReadLine();

            switch (comm)
            {
                case "p":
                    bool success = mediaController.PlayPauseAsync().GetAwaiter().GetResult();
                    if (success) Console.WriteLine("Media Paused/Unpaused");
                    break;
                case "f":
                    success = mediaController.SkipNextAsync().GetAwaiter().GetResult();
                    if (success) Console.WriteLine("Skip forward");
                    break;
                case "b":
                    success = mediaController.SkipPreviousAsync().GetAwaiter().GetResult();
                    if (success) Console.WriteLine("Skip back");
                    break;
                case "i":
                    MediaInfo? info = mediaInfoProvider.GetMediaPropertiesAsync().GetAwaiter().GetResult();

                    if (info is not null)
                    {
                        Console.WriteLine($"App: {info.SourceApp}");
                        Console.WriteLine($"Title: {info.Title}");
                        Console.WriteLine($"Artist: {info.Artist}");
                        Console.WriteLine($"Album: {info.Album}");
                    }
                    break;
                case "q":
                    done = true;
                    break;
                default:
                    break;
            }
        }

        return 0;
    }
}