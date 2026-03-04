namespace Api.Contracts;

public record SchoolDto(
    long? Id,
    string Code,
    string Name,
    string? City,
    bool IsFavorite,
    long? ScopeId);

public record SchoolDtoOut(
    long Id,
    string Code,
    string Name,
    string? City,
    bool IsFavorite,
    long? ScopeId,
    DateTime CreatedAt);
