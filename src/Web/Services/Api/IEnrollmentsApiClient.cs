namespace Web.Services.Api;

public interface IEnrollmentsApiClient
{
    Task<IEnumerable<ApiEnrollment>> GetAllAsync();
}
