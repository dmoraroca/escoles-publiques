namespace Web.Services.Api;
/// <summary>
/// Defines the contract required by i students api client components.
/// </summary>
public interface IStudentsApiClient
{
    Task<IEnumerable<ApiStudent>> GetAllAsync();
    Task<ApiStudent?> GetByIdAsync(long id);
    Task<ApiStudent> CreateAsync(ApiStudentIn dto);
    Task UpdateAsync(long id, ApiStudentIn dto);
    Task DeleteAsync(long id);
}
