using Application.DTOs;
using Application.Interfaces.Search;
using Web.Services.Api;

namespace Web.Services.Search.Adapters;
/// <summary>
/// Encapsulates the functional responsibility of student search source within the application architecture.
/// </summary>
public class StudentSearchSource : IStudentSearchSource
{
    private readonly IStudentsApiClient _studentsApi;
    /// <summary>
    /// Initializes a new instance of the StudentSearchSource class with its required dependencies.
    /// </summary>
    public StudentSearchSource(IStudentsApiClient studentsApi)
    {
        _studentsApi = studentsApi;
    }
    /// <summary>
    /// Retrieves all async and returns it to the caller.
    /// </summary>
    public async Task<IEnumerable<StudentSearchDto>> GetAllAsync()
    {
        var students = await _studentsApi.GetAllAsync();
        return students.Select(s => new StudentSearchDto(
            s.Id,
            s.FirstName,
            s.LastName,
            string.IsNullOrWhiteSpace(s.Email) ? null : s.Email,
            s.SchoolName));
    }
}
