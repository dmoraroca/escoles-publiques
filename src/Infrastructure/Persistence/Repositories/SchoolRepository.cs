using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

/// <summary>
/// Repositori per accedir i gestionar les escoles a la base de dades.
/// </summary>
public class SchoolRepository : ISchoolRepository
{
    private readonly SchoolDbContext _context;

    /// <summary>
    /// Constructor del repositori d'escoles.
    /// </summary>
    public SchoolRepository(SchoolDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retorna totes les escoles ordenades pel nom.
    /// </summary>
    public async Task<IEnumerable<School>> GetAllAsync()
    {
        return await _context.Schools
            .Include(s => s.Scope)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Retorna una escola pel seu identificador, incloent els alumnes.
    /// </summary>
    public async Task<School?> GetByIdAsync(long id)
    {
        return await _context.Schools
            .Include(s => s.Students)
            .Include(s => s.Scope)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    /// <summary>
    /// Retorna una escola pel seu codi.
    /// </summary>
    public async Task<School?> GetByCodeAsync(string code)
    {
        return await _context.Schools
            .Include(s => s.Scope)
            .FirstOrDefaultAsync(s => s.Code == code);
    }

    /// <summary>
    /// Afegeix una nova escola a la base de dades.
    /// </summary>
    public async Task<School> AddAsync(School school)
    {
        _context.Schools.Add(school);
        await _context.SaveChangesAsync();
        return school;
    }

    public async Task UpdateAsync(School school)
    {
        _context.Schools.Update(school);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var school = await _context.Schools.FindAsync(id);
        if (school != null)
        {
            _context.Schools.Remove(school);
            await _context.SaveChangesAsync();
        }
    }
}
