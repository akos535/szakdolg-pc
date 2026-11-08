using System.Text.Json;
using Remote.Core.Input;

namespace Remote.Core.Commands;

internal class MouseClickCommand : ICommand
{
    public string Type => "mouse.click";

    public Task ExecuteAsync(JsonElement payload, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if(payload.ValueKind != JsonValueKind.Object ||
            !payload.TryGetProperty("button", out JsonElement buttonElement) ||
            buttonElement.ValueKind != JsonValueKind.String)
        {
            throw new ArgumentException("Mouse click doesn't have button property");
        }

        string? buttonName = buttonElement.GetString();
        MouseButton button = buttonName?.ToLowerInvariant() switch
        {
            "left" => MouseButton.Left,
            "right" => MouseButton.Right,
            _ => throw new ArgumentException($"Wrong button: {buttonName}")
        };

        InputMaker.SendMouseClick(button);
        return Task.CompletedTask;
    }
}
