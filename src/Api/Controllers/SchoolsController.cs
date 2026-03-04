using Application.Interfaces.Cqrs;
using Application.UseCases.Schools.Commands;
using Application.UseCases.Schools.Queries;
using Api.Contracts;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

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

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var schools = await _getAllSchoolsQuery.HandleAsync(new GetAllSchoolsQuery());
        return Ok(schools.Select(ToDto));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(long id)
    {
        var school = await _getSchoolByIdQuery.HandleAsync(new GetSchoolByIdQuery(id));
        if (school is null) return NotFound();
        return Ok(ToDto(school));
    }

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

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _deleteSchoolCommand.HandleAsync(new DeleteSchoolCommand(id));
        return NoContent();
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
