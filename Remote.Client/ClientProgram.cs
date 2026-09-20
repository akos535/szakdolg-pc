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

        Dictionary<string, string> info = MediaProperties.GetMediaPropertiesAsync();

        foreach (var item in info)
        {
            Console.WriteLine(item.Key + ": " + item.Value);
        }
        return 0;
    }
}
