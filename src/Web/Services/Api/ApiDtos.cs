namespace Web.Services.Api;
/// <summary>
/// Represents values and data structure for api scope.
/// </summary>
public record ApiScope(long Id, string Name);
/// <summary>
/// Represents values and data structure for api student.
/// </summary>
public record ApiStudent(
    long Id,
    long? UserId,
    string FirstName,
    string LastName,
    string Email,
    DateOnly? BirthDate,
    long SchoolId,
    string? SchoolName);
/// <summary>
/// Represents values and data structure for api student in.
/// </summary>
public record ApiStudentIn(
    string FirstName,
    string LastName,
    string Email,
    DateOnly? BirthDate,
    long SchoolId);
/// <summary>
/// Represents values and data structure for api enrollment.
/// </summary>
public record ApiEnrollment(
    long Id,
    long StudentId,
    string StudentName,
    string AcademicYear,
    string? CourseName,
    string Status,
    DateTime EnrolledAt,
    long SchoolId,
    string SchoolName);
/// <summary>
/// Represents values and data structure for api enrollment in.
/// </summary>
public record ApiEnrollmentIn(
    long StudentId,
    string AcademicYear,
    string? CourseName,
    string Status,
    DateTime? EnrolledAt,
    long SchoolId);
/// <summary>
/// Represents values and data structure for api annual fee.
/// </summary>
public record ApiAnnualFee(
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
/// <summary>
/// Represents values and data structure for api annual fee in.
/// </summary>
public record ApiAnnualFeeIn(
    long EnrollmentId,
    decimal Amount,
    string Currency,
    DateOnly DueDate,
    bool IsPaid,
    string? PaymentRef);
