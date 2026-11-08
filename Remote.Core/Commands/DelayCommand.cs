using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Remote.Core.Commands;

internal class DelayCommand : ICommand
{
    public string Type => "delay";

    public async Task ExecuteAsync(JsonElement payload, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if(payload.ValueKind != JsonValueKind.Object ||
            !payload.TryGetProperty("milliseconds", out JsonElement millisecondsElement) ||
            !millisecondsElement.TryGetInt32(out int milliseconds) ||
            milliseconds < 0 || milliseconds > 60000)
        {
            throw new ArgumentException("Delay value incorrect");
        }

        await Task.Delay(milliseconds, cancellationToken);
    }
}
