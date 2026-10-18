namespace Remote.Core.Command_Handlers;
public interface ICommandHandler
{
    Task HandleAsync(
        string message,
        CancellationToken cancellationToken = default);
}
