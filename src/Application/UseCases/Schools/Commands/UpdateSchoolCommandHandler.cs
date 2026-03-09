using Application.Interfaces;
using Application.Interfaces.Cqrs;

namespace Application.UseCases.Schools.Commands;
/// <summary>
/// Encapsulates the functional responsibility of update school command handler within the application architecture.
/// </summary>
public sealed class UpdateSchoolCommandHandler : ICommandHandler<UpdateSchoolCommand, bool>
{
    private readonly ISchoolService _schoolService;
    /// <summary>
    /// Initializes a new instance of the UpdateSchoolCommandHandler class with its required dependencies.
    /// </summary>
    public UpdateSchoolCommandHandler(ISchoolService schoolService)
    {
        _schoolService = schoolService;
    }
    /// <summary>
    /// Handles async and executes the corresponding use case.
    /// </summary>
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
