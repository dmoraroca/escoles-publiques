namespace Web.Services.Api;

public interface IStudentsApiClient
{
    Task<IEnumerable<ApiStudent>> GetAllAsync();
}
