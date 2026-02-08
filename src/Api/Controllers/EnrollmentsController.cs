using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentService _enrollmentService;

    public EnrollmentsController(IEnrollmentService enrollmentService)
    {
        _enrollmentService = enrollmentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var enrollments = await _enrollmentService.GetAllEnrollmentsAsync();
        var result = enrollments.Select(e =>
        {
            var first = e.Student?.User?.FirstName ?? string.Empty;
            var last = e.Student?.User?.LastName ?? string.Empty;
            var studentName = $"{first} {last}".Trim();
            if (string.IsNullOrWhiteSpace(studentName)) studentName = "Desconegut";
            var schoolName = e.School?.Name ?? e.Student?.School?.Name ?? "Desconeguda";

            return new EnrollmentDtoOut(
                e.Id,
                studentName,
                schoolName,
                e.AcademicYear,
                e.EnrolledAt
            );
        });

        return Ok(result);
    }
}

public record EnrollmentDtoOut(long Id, string StudentName, string SchoolName, string AcademicYear, DateTime EnrolledAt);
