namespace Application.Interfaces.Cqrs;
/// <summary>
/// Defines the contract required by i query handler components.
/// </summary>
public interface IQueryHandler<in TQuery, TResult> where TQuery : IQuery<TResult>
{
    Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}
