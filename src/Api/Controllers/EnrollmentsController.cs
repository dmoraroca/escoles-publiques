using Application.Interfaces;
using Domain.DomainExceptions;
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
    private readonly ILogger<EnrollmentsController> _logger;

    public EnrollmentsController(IEnrollmentService enrollmentService, ILogger<EnrollmentsController> logger)
    {
        _enrollmentService = enrollmentService;
        _logger = logger;
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
        try
        {
            var enrollment = await _enrollmentService.GetEnrollmentByIdAsync(id);
            return Ok(ToDto(enrollment!));
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EnrollmentDtoIn dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        try
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
        catch (ValidationException ex)
        {
            var errors = ex.Errors.Count > 0
                ? ex.Errors
                : new Dictionary<string, string[]> { { "Validation", new[] { ex.Message } } };
            return ValidationProblem(new ValidationProblemDetails(errors));
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating enrollment");
            return Problem(detail: ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] EnrollmentDtoIn dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        try
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
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors.Count > 0
                ? ex.Errors
                : new Dictionary<string, string[]> { { "Validation", new[] { ex.Message } } };
            return ValidationProblem(new ValidationProblemDetails(errors));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating enrollment {Id}", id);
            return Problem(detail: ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            await _enrollmentService.DeleteEnrollmentAsync(id);
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting enrollment {Id}", id);
            return Problem(detail: ex.Message);
        }
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

public record EnrollmentDtoIn(
    long StudentId,
    string AcademicYear,
    string? CourseName,
    string Status,
    DateTime? EnrolledAt,
    long SchoolId);

public record EnrollmentDtoOut(
    long Id,
    long StudentId,
    string StudentName,
    string AcademicYear,
    string? CourseName,
    string Status,
    DateTime EnrolledAt,
    long SchoolId,
    string SchoolName);
