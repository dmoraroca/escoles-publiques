using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class AnnualFeeRepository : IAnnualFeeRepository
{
    private readonly SchoolDbContext _context;

    public AnnualFeeRepository(SchoolDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AnnualFee>> GetAllAsync()
    {
        return await _context.AnnualFees
            .Include(af => af.Enrollment)
                .ThenInclude(e => e.Student)
            .OrderByDescending(af => af.DueDate)
            .ToListAsync();
    }

    public async Task<AnnualFee?> GetByIdAsync(long id)
    {
        return await _context.AnnualFees
            .Include(af => af.Enrollment)
                .ThenInclude(e => e.Student)
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
