namespace Web.Services.Api;

public interface IStudentsApiClient
{
    Task<IEnumerable<ApiStudent>> GetAllAsync();
    Task<ApiStudent?> GetByIdAsync(long id);
    Task<ApiStudent> CreateAsync(ApiStudentIn dto);
    Task UpdateAsync(long id, ApiStudentIn dto);
    Task DeleteAsync(long id);
}
