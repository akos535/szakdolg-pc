using System.Text.Json;

using Remote.Core.Input;

namespace Remote.Core.Commands;

internal class KeyPressCommand : ICommand
{
    public string Type => "keyboard.press";

    public Task ExecuteAsync(JsonElement payload, CancellationToken cancellationToken)
    {
        string keyName = $"VK_{payload.GetString()!}";

        if(Enum.TryParse<KeyCode>(keyName, out var keyCode))
        {
            InputMaker.SendKeyPress(keyCode);
            return Task.CompletedTask;
        }

        return Task.CompletedTask;
    }
}
