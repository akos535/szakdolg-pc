using System.Text.Json;

using Remote.Core.Media;

namespace Remote.Core.Commands;

internal sealed class PlayPauseCommand : ICommand
{
    private readonly MediaController _mediaController;
    public string Type => "media.playPause";

    internal PlayPauseCommand(MediaController mediaController)
    {
        _mediaController = mediaController;
    }

    public Task ExecuteAsync(JsonElement json, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return _mediaController.PlayPauseAsync();
    }
}
