using Application.Interfaces.Cqrs;
using Domain.Entities;

namespace Application.UseCases.Schools.Commands;

public sealed record CreateSchoolCommand(
    string Code,
    string Name,
    string? City,
    bool IsFavorite,
    long? ScopeId) : ICommand<School>;
