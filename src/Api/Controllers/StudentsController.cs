using Application.Interfaces;
using Domain.DomainExceptions;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;
    private readonly IUserService _userService;
    private readonly ILogger<StudentsController> _logger;

    public StudentsController(IStudentService studentService, IUserService userService, ILogger<StudentsController> logger)
    {
        _studentService = studentService;
        _userService = userService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var students = await _studentService.GetAllStudentsAsync();
        return Ok(students.Select(ToDto));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(long id)
    {
        try
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            return Ok(ToDto(student!));
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] StudentDtoIn dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        try
        {
            var existingUser = await _userService.GetUserByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                var student = new Student
                {
                    SchoolId = dto.SchoolId,
                    UserId = existingUser.Id
                };
                var created = await _studentService.CreateStudentAsync(student);
                return CreatedAtAction(nameof(Get), new { id = created.Id }, ToDto(created));
            }

            var user = new Domain.Entities.User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                BirthDate = dto.BirthDate
            };
            var newStudent = new Student
            {
                SchoolId = dto.SchoolId
            };
            var createdWithUser = await _studentService.CreateStudentWithUserAsync(user, "user123", newStudent);
            return CreatedAtAction(nameof(Get), new { id = createdWithUser.Id }, ToDto(createdWithUser));
        }
        catch (DuplicateEntityException ex)
        {
            _logger.LogWarning(ex, "Duplicate email on student create");
            return Conflict(ex.Message);
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
            _logger.LogError(ex, "Error creating student");
            return Problem(detail: ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] StudentDtoIn dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        try
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null) return NotFound();

            Domain.Entities.User? user = null;
            if (student.UserId.HasValue)
            {
                user = await _userService.GetUserByIdAsync(student.UserId.Value);
            }

            if (user != null)
            {
                user.FirstName = dto.FirstName;
                user.LastName = dto.LastName;
                user.Email = dto.Email;
                user.BirthDate = dto.BirthDate;

                student.SchoolId = dto.SchoolId;
                await _studentService.UpdateStudentWithUserAsync(student, user);
            }
            else
            {
                student.SchoolId = dto.SchoolId;
                await _studentService.UpdateStudentAsync(student);
            }

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
            _logger.LogError(ex, "Error updating student {Id}", id);
            return Problem(detail: ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            await _studentService.DeleteStudentAsync(id);
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting student {Id}", id);
            return Problem(detail: ex.Message);
        }
    }

    private static StudentDtoOut ToDto(Student student)
    {
        return new StudentDtoOut(
            student.Id,
            student.UserId,
            student.User?.FirstName ?? string.Empty,
            student.User?.LastName ?? string.Empty,
            student.User?.Email ?? string.Empty,
            student.User?.BirthDate,
            student.SchoolId,
            student.School?.Name
        );
    }
}

public record StudentDtoIn(string FirstName, string LastName, string Email, DateOnly? BirthDate, long SchoolId);

public record StudentDtoOut(
    long Id,
    long? UserId,
    string FirstName,
    string LastName,
    string Email,
    DateOnly? BirthDate,
    long SchoolId,
    string? SchoolName);
