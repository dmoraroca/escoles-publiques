namespace Api.Contracts;
/// <summary>
/// Represents values and data structure for school dto.
/// </summary>
public record SchoolDto(
    long? Id,
    string Code,
    string Name,
    string? City,
    bool IsFavorite,
    long? ScopeId);
/// <summary>
/// Represents values and data structure for school dto out.
/// </summary>
public record SchoolDtoOut(
    long Id,
    string Code,
    string Name,
    string? City,
    bool IsFavorite,
    long? ScopeId,
    DateTime CreatedAt);
