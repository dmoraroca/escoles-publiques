using Domain.Entities;

namespace Domain.Interfaces;
/// <summary>
/// Centralizes persistent data access for i scope.
/// </summary>
public interface IScopeRepository
{
    Task<IEnumerable<Scope>> GetAllActiveScopesAsync();
    Task<Scope?> GetScopeByIdAsync(long id);
    Task<Scope?> GetScopeByNameAsync(string name);
}
