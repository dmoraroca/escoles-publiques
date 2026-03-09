using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;
/// <summary>
/// Centralizes persistent data access for annual fee.
/// </summary>
public class AnnualFeeRepository : IAnnualFeeRepository
{
    private readonly SchoolDbContext _context;
            /// <summary>
            /// Initializes a new instance of the AnnualFeeRepository class with its required dependencies.
            /// </summary>
            public AnnualFeeRepository(SchoolDbContext context)
    {
        _context = context;
    }
            /// <summary>
            /// Retrieves all async and returns it to the caller.
            /// </summary>
            public async Task<IEnumerable<AnnualFee>> GetAllAsync()
    {
        return await _context.AnnualFees
            .Include(af => af.Enrollment)
                .ThenInclude(e => e.Student)
                    .ThenInclude(s => s.User)
            .Include(af => af.Enrollment)
                .ThenInclude(e => e.School)
            .OrderByDescending(af => af.DueDate)
            .ToListAsync();
    }
            /// <summary>
            /// Retrieves by id async and returns it to the caller.
            /// </summary>
            public async Task<AnnualFee?> GetByIdAsync(long id)
    {
        return await _context.AnnualFees
            .Include(af => af.Enrollment)
                .ThenInclude(e => e.Student)
                    .ThenInclude(s => s.User)
            .Include(af => af.Enrollment)
                .ThenInclude(e => e.School)
            .FirstOrDefaultAsync(af => af.Id == id);
    }
            /// <summary>
            /// Retrieves by enrollment id async and returns it to the caller.
            /// </summary>
            public async Task<IEnumerable<AnnualFee>> GetByEnrollmentIdAsync(long enrollmentId)
    {
        return await _context.AnnualFees
            .Where(af => af.EnrollmentId == enrollmentId)
            .OrderBy(af => af.DueDate)
            .ToListAsync();
    }
            /// <summary>
            /// Executes the add async operation as part of this component.
            /// </summary>
            public async Task<AnnualFee> AddAsync(AnnualFee annualFee)
    {
        _context.AnnualFees.Add(annualFee);
        await _context.SaveChangesAsync();
        return annualFee;
    }
            /// <summary>
            /// Updates async with the data received in the request.
            /// </summary>
            public async Task UpdateAsync(AnnualFee annualFee)
    {
        _context.AnnualFees.Update(annualFee);
        await _context.SaveChangesAsync();
    }
            /// <summary>
            /// Deletes async from the system in a controlled manner.
            /// </summary>
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
