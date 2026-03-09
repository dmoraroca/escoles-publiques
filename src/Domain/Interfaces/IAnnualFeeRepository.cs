using Domain.Entities;

namespace Domain.Interfaces;
/// <summary>
/// Centralizes persistent data access for i annual fee.
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
