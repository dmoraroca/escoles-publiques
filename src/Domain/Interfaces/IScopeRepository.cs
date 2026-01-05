using Domain.Entities;

namespace Domain.Interfaces;

public interface IScopeRepository
{
    Task<IEnumerable<Scope>> GetAllActiveScopesAsync();
    Task<Scope?> GetScopeByIdAsync(long id);
    Task<Scope?> GetScopeByNameAsync(string name);
}
