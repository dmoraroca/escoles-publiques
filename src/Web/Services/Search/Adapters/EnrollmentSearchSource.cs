using Application.DTOs;
using Application.Interfaces.Search;
using Web.Services.Api;

namespace Web.Services.Search.Adapters;
/// <summary>
/// Encapsulates the functional responsibility of enrollment search source within the application architecture.
/// </summary>
public class EnrollmentSearchSource : IEnrollmentSearchSource
{
    private readonly IEnrollmentsApiClient _enrollmentsApi;
    /// <summary>
    /// Initializes a new instance of the EnrollmentSearchSource class with its required dependencies.
    /// </summary>
    public EnrollmentSearchSource(IEnrollmentsApiClient enrollmentsApi)
    {
        _enrollmentsApi = enrollmentsApi;
    }
    /// <summary>
    /// Retrieves all async and returns it to the caller.
    /// </summary>
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
