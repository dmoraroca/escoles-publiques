using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SchoolsController : ControllerBase
{
    private readonly ISchoolService _schoolService;
    private readonly ILogger<SchoolsController> _logger;

    public SchoolsController(ISchoolService schoolService, ILogger<SchoolsController> logger)
    {
        _schoolService = schoolService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var schools = await _schoolService.GetAllSchoolsAsync();
        return Ok(schools.Select(ToDto));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(long id)
    {
        try
        {
            var school = await _schoolService.GetSchoolByIdAsync(id);
            return Ok(ToDto(school));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "School not found {Id}", id);
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SchoolDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var school = new School
        {
            Code = dto.Code,
            Name = dto.Name,
            City = dto.City,
            IsFavorite = dto.IsFavorite,
            ScopeId = dto.ScopeId
        };

        try
        {
            var created = await _schoolService.CreateSchoolAsync(school);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating school");
            return Problem(detail: ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] SchoolDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        try
        {
            var school = await _schoolService.GetSchoolByIdAsync(id);
            school.Code = dto.Code;
            school.Name = dto.Name;
            school.City = dto.City;
            school.IsFavorite = dto.IsFavorite;
            school.ScopeId = dto.ScopeId;

            await _schoolService.UpdateSchoolAsync(school);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating school {Id}", id);
            return Problem(detail: ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            await _schoolService.DeleteSchoolAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting school {Id}", id);
            return Problem(detail: ex.Message);
        }
    }

    private static SchoolDtoOut ToDto(School school)
    {
        return new SchoolDtoOut(
            school.Id,
            school.Code,
            school.Name,
            school.City,
            school.IsFavorite,
            school.ScopeId,
            school.CreatedAt
        );
    }
}

public record SchoolDto
(
    long? Id,
    string Code,
    string Name,
    string? City,
    bool IsFavorite,
    long? ScopeId
);

public record SchoolDtoOut
(
    long Id,
    string Code,
    string Name,
    string? City,
    bool IsFavorite,
    long? ScopeId,
    DateTime CreatedAt
);
