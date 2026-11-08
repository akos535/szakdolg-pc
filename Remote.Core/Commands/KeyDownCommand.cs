using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

using Remote.Core.Input;

namespace Remote.Core.Commands;

internal class KeyDownCommand : ICommand
{
    public string Type => "keyboard.down";

    public Task ExecuteAsync(JsonElement payload, CancellationToken cancellationToken)
    {
        string keyName = $"VK_{payload.GetString()!}";

        if (Enum.TryParse<KeyCode>(keyName, out var keyCode))
        {
            InputMaker.SendKeyDown(keyCode);
            return Task.CompletedTask;
        }

        return Task.CompletedTask;
    }
}
