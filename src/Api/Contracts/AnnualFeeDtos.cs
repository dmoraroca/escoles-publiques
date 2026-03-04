namespace Api.Contracts;

public record AnnualFeeDtoIn(
    long EnrollmentId,
    decimal Amount,
    string Currency,
    DateOnly DueDate,
    bool IsPaid,
    string? PaymentRef);

public record AnnualFeeDtoOut(
    long Id,
    long EnrollmentId,
    string EnrollmentInfo,
    string StudentName,
    string AcademicYear,
    string? CourseName,
    decimal Amount,
    string Currency,
    DateOnly DueDate,
    DateTime? PaidAt,
    string? PaymentRef,
    long? SchoolId,
    string? SchoolName);
