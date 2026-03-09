using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;
/// <summary>
/// Centralizes persistent data access for school.
/// </summary>
public class SchoolRepository : ISchoolRepository
{
    private readonly SchoolDbContext _context;
    /// <summary>
    /// Initializes a new instance of the SchoolRepository class with its required dependencies.
    /// </summary>
    public SchoolRepository(SchoolDbContext context)
    {
        _context = context;
    }
    /// <summary>
    /// Retrieves all async and returns it to the caller.
    /// </summary>
    public async Task<IEnumerable<School>> GetAllAsync()
    {
        return await _context.Schools
            .Include(s => s.Scope)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }
    /// <summary>
    /// Retrieves by id async and returns it to the caller.
    /// </summary>
    public async Task<School?> GetByIdAsync(long id)
    {
        return await _context.Schools
            .Include(s => s.Students)
            .Include(s => s.Scope)
            .FirstOrDefaultAsync(s => s.Id == id);
    }
    /// <summary>
    /// Retrieves by code async and returns it to the caller.
    /// </summary>
    public async Task<School?> GetByCodeAsync(string code)
    {
        return await _context.Schools
            .Include(s => s.Scope)
            .FirstOrDefaultAsync(s => s.Code == code);
    }
    /// <summary>
    /// Executes the add async operation as part of this component.
    /// </summary>
    public async Task<School> AddAsync(School school)
    {
        _context.Schools.Add(school);
        await _context.SaveChangesAsync();
        return school;
    }
    /// <summary>
    /// Updates async with the data received in the request.
    /// </summary>
    public async Task UpdateAsync(School school)
    {
        _context.Schools.Update(school);
        await _context.SaveChangesAsync();
    }
    /// <summary>
    /// Deletes async from the system in a controlled manner.
    /// </summary>
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
