namespace Web.Services.Api;
/// <summary>
/// Defines the contract required by i annual fees api client components.
/// </summary>
public interface IAnnualFeesApiClient
{
    Task<IEnumerable<ApiAnnualFee>> GetAllAsync();
    Task<ApiAnnualFee?> GetByIdAsync(long id);
    Task<ApiAnnualFee> CreateAsync(ApiAnnualFeeIn dto);
    Task UpdateAsync(long id, ApiAnnualFeeIn dto);
    Task DeleteAsync(long id);
}
