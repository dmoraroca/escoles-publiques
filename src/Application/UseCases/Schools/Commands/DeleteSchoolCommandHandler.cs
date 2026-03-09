using Application.Interfaces;
using Application.Interfaces.Cqrs;

namespace Application.UseCases.Schools.Commands;
/// <summary>
/// Encapsulates the functional responsibility of delete school command handler within the application architecture.
/// </summary>
public sealed class DeleteSchoolCommandHandler : ICommandHandler<DeleteSchoolCommand, bool>
{
    private readonly ISchoolService _schoolService;
            /// <summary>
            /// Initializes a new instance of the DeleteSchoolCommandHandler class with its required dependencies.
            /// </summary>
            public DeleteSchoolCommandHandler(ISchoolService schoolService)
    {
        _schoolService = schoolService;
    }
            /// <summary>
            /// Handles async and executes the corresponding use case.
            /// </summary>
            public async Task<bool> HandleAsync(DeleteSchoolCommand command, CancellationToken cancellationToken = default)
    {
        await _schoolService.DeleteSchoolAsync(command.Id);
        return true;
    }
}
