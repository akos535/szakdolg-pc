using System.Text.Json;

using Remote.Core.Commands;

namespace Remote.Core.Command_Handlers;

internal class CommandHandler : ICommandHandler
{
    private readonly Dictionary<string, ICommand> _commands;

    public CommandHandler(IEnumerable<ICommand> command)
    {
        _commands = command.ToDictionary(
            command => command.Type,
            StringComparer.Ordinal);
    }

    public async Task HandleAsync(string message, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Handler input: length={message.Length}, text=[{message}]");

        using JsonDocument document = JsonDocument.Parse(message);
        var root = document.RootElement;

        if(root.ValueKind != JsonValueKind.Object ||
            !root.TryGetProperty("type", out var typeElement) ||
            typeElement.ValueKind != JsonValueKind.String)
        {
            throw new ArgumentException("no type.");
        }

        string type = typeElement.GetString()!;

        if(!_commands.TryGetValue(type, out var command))
        {
            throw new ArgumentException("bad command.");
        }

        root.TryGetProperty("payload", out var payload);

        await command.ExecuteAsync(payload, cancellationToken);
    }
}
