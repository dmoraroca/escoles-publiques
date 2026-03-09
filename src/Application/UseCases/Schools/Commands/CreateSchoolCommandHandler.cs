using Application.Interfaces;
using Application.Interfaces.Cqrs;
using Domain.Entities;

namespace Application.UseCases.Schools.Commands;
/// <summary>
/// Encapsulates the functional responsibility of create school command handler within the application architecture.
/// </summary>
public sealed class CreateSchoolCommandHandler : ICommandHandler<CreateSchoolCommand, School>
{
    private readonly ISchoolService _schoolService;
            /// <summary>
            /// Initializes a new instance of the CreateSchoolCommandHandler class with its required dependencies.
            /// </summary>
            public CreateSchoolCommandHandler(ISchoolService schoolService)
    {
        _schoolService = schoolService;
    }
            /// <summary>
            /// Handles async and executes the corresponding use case.
            /// </summary>
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
