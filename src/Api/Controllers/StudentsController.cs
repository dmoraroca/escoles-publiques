using Application.Interfaces;
using Api.Contracts;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace Api.Controllers;

/// <summary>
/// Exposes HTTP endpoints to manage students workflows.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;
    private readonly IUserService _userService;
            /// <summary>
            /// Initializes a new instance of the StudentsController class with its required dependencies.
            /// </summary>
            public StudentsController(IStudentService studentService, IUserService userService)
    {
        _studentService = studentService;
        _userService = userService;
    }
    /// <summary>
    /// Retrieves all and returns it to the caller.
    /// </summary>
            [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var students = await _studentService.GetAllStudentsAsync();
        return Ok(students.Select(ToDto));
    }
    /// <summary>
    /// Retrieves the requested data and returns it to the caller.
    /// </summary>
            [HttpGet("{id}")]
    public async Task<IActionResult> Get(long id)
    {
        var student = await _studentService.GetStudentByIdAsync(id);
        return Ok(ToDto(student!));
    }
    /// <summary>
    /// Creates a new resource by applying the required business rules.
    /// </summary>
            [HttpPost]
    public async Task<IActionResult> Create([FromBody] StudentDtoIn dto)
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
        var configuredPassword = Environment.GetEnvironmentVariable("ESCOLES_DEFAULT_STUDENT_PASSWORD");
        var initialPassword = string.IsNullOrWhiteSpace(configuredPassword)
            ? Convert.ToHexString(RandomNumberGenerator.GetBytes(24))
            : configuredPassword;
        var createdWithUser = await _studentService.CreateStudentWithUserAsync(user, initialPassword, newStudent);
        return CreatedAtAction(nameof(Get), new { id = createdWithUser.Id }, ToDto(createdWithUser));
    }
    /// <summary>
    /// Updates the target resource with the data received in the request.
    /// </summary>
            [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] StudentDtoIn dto)
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
    /// <summary>
    /// Deletes the target resource from the system in a controlled manner.
    /// </summary>
            [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _studentService.DeleteStudentAsync(id);
        return NoContent();
    }
            /// <summary>
            /// Maps data for to dto between application layers.
            /// </summary>
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
