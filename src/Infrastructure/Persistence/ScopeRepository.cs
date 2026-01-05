using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class ScopeRepository : IScopeRepository
{
    private readonly SchoolDbContext _context;

    public ScopeRepository(SchoolDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Scope>> GetAllActiveScopesAsync()
    {
        return await _context.Set<Scope>()
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Scope?> GetScopeByIdAsync(long id)
    {
        return await _context.Set<Scope>()
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Scope?> GetScopeByNameAsync(string name)
    {
        return await _context.Set<Scope>()
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Name == name);
    }
}
