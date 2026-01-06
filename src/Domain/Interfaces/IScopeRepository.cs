using Domain.Entities;

namespace Domain.Interfaces;

/// <summary>
/// Interfície de repositori per gestionar àmbits funcionals al domini.
/// </summary>
public interface IScopeRepository
{
    Task<IEnumerable<Scope>> GetAllActiveScopesAsync();
    Task<Scope?> GetScopeByIdAsync(long id);
    Task<Scope?> GetScopeByNameAsync(string name);
}
