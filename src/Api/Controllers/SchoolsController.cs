using Application.Interfaces.Cqrs;
using Application.UseCases.Schools.Commands;
using Application.UseCases.Schools.Queries;
using Api.Contracts;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Exposes HTTP endpoints to manage schools workflows.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SchoolsController : ControllerBase
{
    private readonly IQueryHandler<GetAllSchoolsQuery, IEnumerable<School>> _getAllSchoolsQuery;
    private readonly IQueryHandler<GetSchoolByIdQuery, School?> _getSchoolByIdQuery;
    private readonly ICommandHandler<CreateSchoolCommand, School> _createSchoolCommand;
    private readonly ICommandHandler<UpdateSchoolCommand, bool> _updateSchoolCommand;
    private readonly ICommandHandler<DeleteSchoolCommand, bool> _deleteSchoolCommand;

    public SchoolsController(
        IQueryHandler<GetAllSchoolsQuery, IEnumerable<School>> getAllSchoolsQuery,
        IQueryHandler<GetSchoolByIdQuery, School?> getSchoolByIdQuery,
        ICommandHandler<CreateSchoolCommand, School> createSchoolCommand,
        ICommandHandler<UpdateSchoolCommand, bool> updateSchoolCommand,
        ICommandHandler<DeleteSchoolCommand, bool> deleteSchoolCommand)
    {
        _getAllSchoolsQuery = getAllSchoolsQuery;
        _getSchoolByIdQuery = getSchoolByIdQuery;
        _createSchoolCommand = createSchoolCommand;
        _updateSchoolCommand = updateSchoolCommand;
        _deleteSchoolCommand = deleteSchoolCommand;
    }
    /// <summary>
    /// Retrieves all and returns it to the caller.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var schools = await _getAllSchoolsQuery.HandleAsync(new GetAllSchoolsQuery());
        return Ok(schools.Select(ToDto));
    }
    /// <summary>
    /// Retrieves the requested data and returns it to the caller.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(long id)
    {
        var school = await _getSchoolByIdQuery.HandleAsync(new GetSchoolByIdQuery(id));
        if (school is null) return NotFound();
        return Ok(ToDto(school));
    }
    /// <summary>
    /// Creates a new resource by applying the required business rules.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SchoolDto dto)
    {
        var created = await _createSchoolCommand.HandleAsync(new CreateSchoolCommand(
            dto.Code,
            dto.Name,
            dto.City,
            dto.IsFavorite,
            dto.ScopeId));
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }
    /// <summary>
    /// Updates the target resource with the data received in the request.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] SchoolDto dto)
    {
        var updated = await _updateSchoolCommand.HandleAsync(new UpdateSchoolCommand(
            id,
            dto.Code,
            dto.Name,
            dto.City,
            dto.IsFavorite,
            dto.ScopeId));
        if (!updated) return NotFound();
        return NoContent();
    }
    /// <summary>
    /// Deletes the target resource from the system in a controlled manner.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _deleteSchoolCommand.HandleAsync(new DeleteSchoolCommand(id));
        return NoContent();
    }
    /// <summary>
    /// Maps data for to dto between application layers.
    /// </summary>
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
