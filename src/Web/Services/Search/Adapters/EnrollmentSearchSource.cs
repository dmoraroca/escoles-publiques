using Application.DTOs;
using Application.Interfaces.Search;
using Web.Services.Api;

namespace Web.Services.Search.Adapters;

public class EnrollmentSearchSource : IEnrollmentSearchSource
{
    private readonly IEnrollmentsApiClient _enrollmentsApi;

    public EnrollmentSearchSource(IEnrollmentsApiClient enrollmentsApi)
    {
        _enrollmentsApi = enrollmentsApi;
    }

    public async Task<IEnumerable<EnrollmentSearchDto>> GetAllAsync()
    {
        var enrollments = await _enrollmentsApi.GetAllAsync();
        return enrollments.Select(e => new EnrollmentSearchDto(
            e.Id,
            e.StudentName,
            e.SchoolName,
            e.AcademicYear,
            e.EnrolledAt));
    }
}
