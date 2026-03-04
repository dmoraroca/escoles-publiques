using Application.Interfaces.Cqrs;

namespace Application.UseCases.Schools.Commands;

public sealed record UpdateSchoolCommand(
    long Id,
    string Code,
    string Name,
    string? City,
    bool IsFavorite,
    long? ScopeId) : ICommand<bool>;
