namespace Application.DTOs;

public record ScopeLookupDto(long Id, string Name);

public record StudentSearchDto(
    long Id,
    string FirstName,
    string LastName,
    string? Email,
    string? SchoolName);

public record EnrollmentSearchDto(
    long Id,
    string StudentName,
    string SchoolName,
    string AcademicYear,
    DateTime EnrolledAt);

public record AnnualFeeSearchDto(
    long Id,
    string StudentName,
    decimal Amount,
    string Currency,
    DateOnly DueDate,
    DateTime? PaidAt);
