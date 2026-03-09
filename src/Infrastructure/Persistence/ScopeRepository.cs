using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence;
/// <summary>
/// Centralizes persistent data access for scope.
/// </summary>
public class ScopeRepository : IScopeRepository
{
    private readonly SchoolDbContext _context;
    private readonly ILogger<ScopeRepository> _logger;
            /// <summary>
            /// Initializes a new instance of the ScopeRepository class with its required dependencies.
            /// </summary>
            public ScopeRepository(SchoolDbContext context, ILogger<ScopeRepository> logger)
    {
        _context = context;
        _logger = logger;
    }
            /// <summary>
            /// Retrieves all active scopes async and returns it to the caller.
            /// </summary>
            public async Task<IEnumerable<Scope>> GetAllActiveScopesAsync()
    {
        var scopes = await _context.Set<Scope>()
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .AsNoTracking()
            .ToListAsync();

        _logger.LogInformation("Scopes loaded: {Count}. DB={Database}", scopes.Count, _context.Database.GetDbConnection().Database);
        return scopes;
    }
            /// <summary>
            /// Retrieves scope by id async and returns it to the caller.
            /// </summary>
            public async Task<Scope?> GetScopeByIdAsync(long id)
    {
        return await _context.Set<Scope>()
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);
    }
            /// <summary>
            /// Retrieves scope by name async and returns it to the caller.
            /// </summary>
            public async Task<Scope?> GetScopeByNameAsync(string name)
    {
        return await _context.Set<Scope>()
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Name == name);
    }
}
