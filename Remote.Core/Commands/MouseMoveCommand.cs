using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

using Remote.Core.Input;

namespace Remote.Core.Commands;

internal class MouseMoveCommand : ICommand
{
    public string Type => "mouse.move";

    public Task ExecuteAsync(JsonElement payload, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if(payload.ValueKind != JsonValueKind.Object ||
            !payload.TryGetProperty("dx", out var x) || !payload.TryGetProperty("dy", out var y) ||
            x.ValueKind != JsonValueKind.Number || y.ValueKind != JsonValueKind.Number ||
            !x.TryGetDouble(out double dx) || !y.TryGetDouble(out double dy))
        {
            throw new ArgumentException("invalid dx, dy");
        }

        InputMaker.MouseMoveRelative(dx, dy);

        return Task.CompletedTask;
    }
}
