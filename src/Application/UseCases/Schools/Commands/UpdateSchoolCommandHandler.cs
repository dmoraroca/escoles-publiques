using Application.Interfaces;
using Application.Interfaces.Cqrs;

namespace Application.UseCases.Schools.Commands;

public sealed class UpdateSchoolCommandHandler : ICommandHandler<UpdateSchoolCommand, bool>
{
    private readonly ISchoolService _schoolService;

    public UpdateSchoolCommandHandler(ISchoolService schoolService)
    {
        _schoolService = schoolService;
    }

    public async Task<bool> HandleAsync(UpdateSchoolCommand command, CancellationToken cancellationToken = default)
    {
        var school = await _schoolService.GetSchoolByIdAsync(command.Id);
        if (school is null)
        {
            return false;
        }

        school.Code = command.Code;
        school.Name = command.Name;
        school.City = command.City;
        school.IsFavorite = command.IsFavorite;
        school.ScopeId = command.ScopeId;

        await _schoolService.UpdateSchoolAsync(school);
        return true;
    }
}
