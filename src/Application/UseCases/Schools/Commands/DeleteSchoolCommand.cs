using Application.Interfaces.Cqrs;

namespace Application.UseCases.Schools.Commands;

public sealed record DeleteSchoolCommand(long Id) : ICommand<bool>;
