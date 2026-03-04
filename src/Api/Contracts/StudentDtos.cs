namespace Api.Contracts;

public record StudentDtoIn(string FirstName, string LastName, string Email, DateOnly? BirthDate, long SchoolId);

public record StudentDtoOut(
    long Id,
    long? UserId,
    string FirstName,
    string LastName,
    string Email,
    DateOnly? BirthDate,
    long SchoolId,
    string? SchoolName);
