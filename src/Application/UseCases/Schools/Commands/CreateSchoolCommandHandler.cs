using Application.Interfaces;
using Application.Interfaces.Cqrs;
using Domain.Entities;

namespace Application.UseCases.Schools.Commands;

public sealed class CreateSchoolCommandHandler : ICommandHandler<CreateSchoolCommand, School>
{
    private readonly ISchoolService _schoolService;

    public CreateSchoolCommandHandler(ISchoolService schoolService)
    {
        _schoolService = schoolService;
    }

    public async Task<School> HandleAsync(CreateSchoolCommand command, CancellationToken cancellationToken = default)
    {
        var school = new School
        {
            Code = command.Code,
            Name = command.Name,
            City = command.City,
            IsFavorite = command.IsFavorite,
            ScopeId = command.ScopeId
        };

        return await _schoolService.CreateSchoolAsync(school);
    }
}
