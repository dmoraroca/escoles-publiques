using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class SchoolRepository : ISchoolRepository
{
    private readonly SchoolDbContext _context;

    public SchoolRepository(SchoolDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<School>> GetAllAsync()
    {
        return await _context.Schools
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<School?> GetByIdAsync(long id)
    {
        return await _context.Schools
            .Include(s => s.Students)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<School?> GetByCodeAsync(string code)
    {
        return await _context.Schools
            .FirstOrDefaultAsync(s => s.Code == code);
    }

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
