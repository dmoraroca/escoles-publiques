using Application.Interfaces;
using Application.Interfaces.Cqrs;

namespace Application.UseCases.Schools.Commands;

public sealed class DeleteSchoolCommandHandler : ICommandHandler<DeleteSchoolCommand, bool>
{
    private readonly ISchoolService _schoolService;

    public DeleteSchoolCommandHandler(ISchoolService schoolService)
    {
        _schoolService = schoolService;
    }

    public async Task<bool> HandleAsync(DeleteSchoolCommand command, CancellationToken cancellationToken = default)
    {
        await _schoolService.DeleteSchoolAsync(command.Id);
        return true;
    }
}
