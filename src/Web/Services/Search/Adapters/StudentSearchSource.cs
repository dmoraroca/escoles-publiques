using Application.DTOs;
using Application.Interfaces.Search;
using Web.Services.Api;

namespace Web.Services.Search.Adapters;

public class StudentSearchSource : IStudentSearchSource
{
    private readonly IStudentsApiClient _studentsApi;

    public StudentSearchSource(IStudentsApiClient studentsApi)
    {
        _studentsApi = studentsApi;
    }

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
