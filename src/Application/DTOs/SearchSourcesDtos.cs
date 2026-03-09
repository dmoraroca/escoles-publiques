namespace Application.DTOs;
/// <summary>
/// Represents values and data structure for scope lookup dto.
/// </summary>
public record ScopeLookupDto(long Id, string Name);
/// <summary>
/// Represents values and data structure for student search dto.
/// </summary>
public record StudentSearchDto(
    long Id,
    string FirstName,
    string LastName,
    string? Email,
    string? SchoolName);
/// <summary>
/// Represents values and data structure for enrollment search dto.
/// </summary>
public record EnrollmentSearchDto(
    long Id,
    string StudentName,
    string SchoolName,
    string AcademicYear,
    DateTime EnrolledAt);
/// <summary>
/// Represents values and data structure for annual fee search dto.
/// </summary>
public record AnnualFeeSearchDto(
    long Id,
    string StudentName,
    decimal Amount,
    string Currency,
    DateOnly DueDate,
    DateTime? PaidAt);
