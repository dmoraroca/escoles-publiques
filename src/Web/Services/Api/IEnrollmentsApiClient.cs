namespace Web.Services.Api;
/// <summary>
/// Defines the contract required by i enrollments api client components.
/// </summary>
public interface IEnrollmentsApiClient
{
    Task<IEnumerable<ApiEnrollment>> GetAllAsync();
    Task<ApiEnrollment?> GetByIdAsync(long id);
    Task<ApiEnrollment> CreateAsync(ApiEnrollmentIn dto);
    Task UpdateAsync(long id, ApiEnrollmentIn dto);
    Task DeleteAsync(long id);
}
