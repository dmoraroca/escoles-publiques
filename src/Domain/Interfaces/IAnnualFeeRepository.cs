using Domain.Entities;

namespace Domain.Interfaces;

public interface IAnnualFeeRepository
{
    Task<IEnumerable<AnnualFee>> GetAllAsync();
    Task<AnnualFee?> GetByIdAsync(long id);
    Task<IEnumerable<AnnualFee>> GetByEnrollmentIdAsync(long enrollmentId);
    Task<AnnualFee> AddAsync(AnnualFee annualFee);
    Task UpdateAsync(AnnualFee annualFee);
    Task DeleteAsync(long id);
}
