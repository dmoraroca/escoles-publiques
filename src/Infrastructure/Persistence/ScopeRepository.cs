using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

/// <summary>
/// Repository for accessing Scope entities in the database.
/// </summary>
public class ScopeRepository : IScopeRepository
{
    private readonly SchoolDbContext _context;

    /// <summary>
    /// Initializes a new instance of the ScopeRepository class.
    /// </summary>
    /// <param name="context">The database context to use.</param>
    public ScopeRepository(SchoolDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all active scopes from the database, ordered by name.
    /// </summary>
    /// <returns>Enumerable of active scopes.</returns>
    public async Task<IEnumerable<Scope>> GetAllActiveScopesAsync()
    {
        return await _context.Set<Scope>()
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves a scope by its unique identifier.
    /// </summary>
    /// <param name="id">Scope identifier.</param>
    /// <returns>The scope if found; otherwise, null.</returns>
    public async Task<Scope?> GetScopeByIdAsync(long id)
    {
        return await _context.Set<Scope>()
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    /// <summary>
    /// Retrieves a scope by its name.
    /// </summary>
    /// <param name="name">Scope name.</param>
    /// <returns>The scope if found; otherwise, null.</returns>
    public async Task<Scope?> GetScopeByNameAsync(string name)
    {
        return await _context.Set<Scope>()
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Name == name);
    }
}
