using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

/// <summary>
/// Repositori per gestionar les quotes anuals a la base de dades.
/// </summary>
public class AnnualFeeRepository : IAnnualFeeRepository
{
    private readonly SchoolDbContext _context;

    /// <summary>
    /// Constructor del repositori de quotes anuals.
    /// </summary>
    public AnnualFeeRepository(SchoolDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retorna totes les quotes anuals amb inscripcions i alumnes.
    /// </summary>
    public async Task<IEnumerable<AnnualFee>> GetAllAsync()
    {
        return await _context.AnnualFees
            .Include(af => af.Enrollment)
                .ThenInclude(e => e.Student)
                    .ThenInclude(s => s.User)
            .Include(af => af.Enrollment)
                .ThenInclude(e => e.Student)
                    .ThenInclude(s => s.School)
            .OrderByDescending(af => af.DueDate)
            .ToListAsync();
    }

    public async Task<AnnualFee?> GetByIdAsync(long id)
    {
        return await _context.AnnualFees
            .Include(af => af.Enrollment)
                .ThenInclude(e => e.Student)
                    .ThenInclude(s => s.User)
            .FirstOrDefaultAsync(af => af.Id == id);
    }

    public async Task<IEnumerable<AnnualFee>> GetByEnrollmentIdAsync(long enrollmentId)
    {
        return await _context.AnnualFees
            .Where(af => af.EnrollmentId == enrollmentId)
            .OrderBy(af => af.DueDate)
            .ToListAsync();
    }

    public async Task<AnnualFee> AddAsync(AnnualFee annualFee)
    {
        _context.AnnualFees.Add(annualFee);
        await _context.SaveChangesAsync();
        return annualFee;
    }

    public async Task UpdateAsync(AnnualFee annualFee)
    {
        _context.AnnualFees.Update(annualFee);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var annualFee = await _context.AnnualFees.FindAsync(id);
        if (annualFee != null)
        {
            _context.AnnualFees.Remove(annualFee);
            await _context.SaveChangesAsync();
        }
    }
}
