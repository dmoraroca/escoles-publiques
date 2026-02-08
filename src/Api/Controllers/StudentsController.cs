using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentsController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var students = await _studentService.GetAllStudentsAsync();
        var result = students.Select(s => new StudentDtoOut(
            s.Id,
            s.User?.FirstName ?? string.Empty,
            s.User?.LastName ?? string.Empty,
            s.User?.Email ?? string.Empty,
            s.School?.Name
        ));
        return Ok(result);
    }
}

public record StudentDtoOut(long Id, string FirstName, string LastName, string Email, string? SchoolName);
