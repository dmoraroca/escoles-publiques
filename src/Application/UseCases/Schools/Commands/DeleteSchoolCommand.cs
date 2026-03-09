using Application.Interfaces.Cqrs;

namespace Application.UseCases.Schools.Commands;
/// <summary>
/// Represents values and data structure for delete school command.
/// </summary>
public sealed record DeleteSchoolCommand(long Id) : ICommand<bool>;
