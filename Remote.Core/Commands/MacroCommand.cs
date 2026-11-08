using System.Text.Json;

using Remote.Core.Command_Handlers;

namespace Remote.Core.Commands;

internal class MacroCommand : ICommand
{
    private const int MaximumSteps = 100;
    private readonly ICommandHandler _stepHandler;

    public MacroCommand(IEnumerable<ICommand> supportedCommands)
    {
        _stepHandler = new CommandHandler(supportedCommands);
    }
    public string Type => "macro.execute";

    public async Task ExecuteAsync(JsonElement payload, CancellationToken cancellationToken)
    {
        if(payload.ValueKind != JsonValueKind.Object || 
            !payload.TryGetProperty("steps", out JsonElement steps)|| 
            steps.ValueKind != JsonValueKind.Array)
        {
            throw new ArgumentException("Not an array");
        } 

        if(steps.GetArrayLength() > MaximumSteps)
        {
            throw new ArgumentException("Too many steps");
        }

        foreach(JsonElement step in steps.EnumerateArray())
        {
            cancellationToken.ThrowIfCancellationRequested();

            if(step.ValueKind != JsonValueKind.Object ||
                !step.TryGetProperty("type", out JsonElement typeElement) ||
                typeElement.ValueKind != JsonValueKind.String)
            {
                throw new ArgumentException("Step is not a valid command.");
            }

            await _stepHandler.HandleAsync(step.GetRawText(), cancellationToken);
        }
    }
}
