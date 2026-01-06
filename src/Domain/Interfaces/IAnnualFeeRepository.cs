using Domain.Entities;

namespace Domain.Interfaces;

/// <summary>
/// Interfície de repositori per gestionar quotes anuals al domini.
/// </summary>
public interface IAnnualFeeRepository
{
    Task<IEnumerable<AnnualFee>> GetAllAsync();
    Task<AnnualFee?> GetByIdAsync(long id);
    Task<IEnumerable<AnnualFee>> GetByEnrollmentIdAsync(long enrollmentId);
    Task<AnnualFee> AddAsync(AnnualFee annualFee);
    Task UpdateAsync(AnnualFee annualFee);
    Task DeleteAsync(long id);
}
