using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Remote.Core.Commands;

internal interface ICommand
{
    string Type { get; }

    Task ExecuteAsync(
        JsonElement json,
        CancellationToken cancellationToken);
}
