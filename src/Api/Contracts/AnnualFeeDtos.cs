namespace Api.Contracts;
/// <summary>
/// Represents values and data structure for annual fee dto in.
/// </summary>
public record AnnualFeeDtoIn(
    long EnrollmentId,
    decimal Amount,
    string Currency,
    DateOnly DueDate,
    bool IsPaid,
    string? PaymentRef);
/// <summary>
/// Represents values and data structure for annual fee dto out.
/// </summary>
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
