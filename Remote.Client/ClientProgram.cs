using System.Text;

using Remote.Server;

namespace Remote.Client;


internal class ClientProgram
{
    public static int Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        ServerRunner.Run();

        return 0;
    }
}