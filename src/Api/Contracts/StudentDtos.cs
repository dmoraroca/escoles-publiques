namespace Api.Contracts;
/// <summary>
/// Represents values and data structure for student dto in.
/// </summary>
public record StudentDtoIn(string FirstName, string LastName, string Email, DateOnly? BirthDate, long SchoolId);
/// <summary>
/// Represents values and data structure for student dto out.
/// </summary>
public record StudentDtoOut(
    long Id,
    long? UserId,
    string FirstName,
    string LastName,
    string Email,
    DateOnly? BirthDate,
    long SchoolId,
    string? SchoolName);
