using System.Text.Json;

using Remote.Core.Media;

namespace Remote.Core.Commands;

internal class NextCommand : ICommand
{
    private readonly MediaController _mediaController;
    public string Type => "media.next";

    internal NextCommand(MediaController mediaController)
    {
        _mediaController = mediaController;
    }

    public Task ExecuteAsync(JsonElement json, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return _mediaController.SkipNextAsync();
    }
}
