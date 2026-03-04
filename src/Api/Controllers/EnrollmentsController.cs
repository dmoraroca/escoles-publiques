using Application.Interfaces;
using Api.Contracts;
using Domain.Entities;
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
        return Ok(enrollments.Select(ToDto));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(long id)
    {
        var enrollment = await _enrollmentService.GetEnrollmentByIdAsync(id);
        return Ok(ToDto(enrollment!));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EnrollmentDtoIn dto)
    {
        var enrollment = new Enrollment
        {
            StudentId = dto.StudentId,
            AcademicYear = dto.AcademicYear,
            CourseName = dto.CourseName,
            Status = dto.Status,
            SchoolId = dto.SchoolId
        };
        var created = await _enrollmentService.CreateEnrollmentAsync(enrollment);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, ToDto(created));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] EnrollmentDtoIn dto)
    {
        var enrollment = await _enrollmentService.GetEnrollmentByIdAsync(id);
        if (enrollment == null) return NotFound();

        enrollment.StudentId = dto.StudentId;
        enrollment.AcademicYear = dto.AcademicYear;
        enrollment.CourseName = dto.CourseName;
        enrollment.Status = dto.Status;
        enrollment.SchoolId = dto.SchoolId;
        if (dto.EnrolledAt.HasValue)
        {
            enrollment.EnrolledAt = dto.EnrolledAt.Value;
        }

        await _enrollmentService.UpdateEnrollmentAsync(enrollment);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _enrollmentService.DeleteEnrollmentAsync(id);
        return NoContent();
    }

    private static EnrollmentDtoOut ToDto(Enrollment e)
    {
        var first = e.Student?.User?.FirstName ?? string.Empty;
        var last = e.Student?.User?.LastName ?? string.Empty;
        var studentName = $"{first} {last}".Trim();
        if (string.IsNullOrWhiteSpace(studentName)) studentName = "Alumne desconegut";
        var schoolName = e.School?.Name ?? e.Student?.School?.Name ?? string.Empty;

        return new EnrollmentDtoOut(
            e.Id,
            e.StudentId,
            studentName,
            e.AcademicYear,
            e.CourseName,
            e.Status,
            e.EnrolledAt,
            e.SchoolId,
            schoolName
        );
    }
}
