namespace Web.Services.Api;

public interface IEnrollmentsApiClient
{
    Task<IEnumerable<ApiEnrollment>> GetAllAsync();
    Task<ApiEnrollment?> GetByIdAsync(long id);
    Task<ApiEnrollment> CreateAsync(ApiEnrollmentIn dto);
    Task UpdateAsync(long id, ApiEnrollmentIn dto);
    Task DeleteAsync(long id);
}
