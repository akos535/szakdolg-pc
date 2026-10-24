using Remote.Core.Command_Handlers;
using Remote.Core.Commands;
using Remote.Core.Media;

namespace Remote.Server;

public static class ServerRunner
{
    public static void Run()
    {
        RunAsync().GetAwaiter().GetResult();
    }

    private static async Task RunAsync()
    {
        MediaSessionProvider sessionProvider = MediaSessionProvider.CreateAsync().GetAwaiter().GetResult();
        
        MediaInfoProvider mediaInfoProvider = new(sessionProvider);

        CommandHandler handler = new CommandHandler(
            new ICommand[]
            {
                new KeyPressCommand()
            });


        await using var server = new RemoteServer(handler);

        await server.StartAsync();

        Console.WriteLine("Server started. Press Enter to stop.");
        await Task.Run(() => Console.ReadLine());
    }
}