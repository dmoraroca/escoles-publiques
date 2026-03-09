namespace Application.Interfaces.Cqrs;
/// <summary>
/// Defines a handler contract for executing a command and returning its result.
/// </summary>
public interface ICommandHandler<in TCommand, TResult> where TCommand : ICommand<TResult>
{
    Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}
