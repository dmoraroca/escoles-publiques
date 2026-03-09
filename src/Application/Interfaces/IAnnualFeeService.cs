using Domain.Entities;

namespace Application.Interfaces;
/// <summary>
/// Implements application logic for i annual fee operations.
/// </summary>
public interface IAnnualFeeService
{
    Task<IEnumerable<AnnualFee>> GetAllAnnualFeesAsync();
    Task<AnnualFee?> GetAnnualFeeByIdAsync(long id);
    Task<IEnumerable<AnnualFee>> GetAnnualFeesByEnrollmentIdAsync(long enrollmentId);
    Task<AnnualFee> CreateAnnualFeeAsync(AnnualFee annualFee);
    Task UpdateAnnualFeeAsync(AnnualFee annualFee);
    Task DeleteAnnualFeeAsync(long id);
}
