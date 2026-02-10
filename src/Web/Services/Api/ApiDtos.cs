namespace Web.Services.Api;

public record ApiScope(long Id, string Name);

public record ApiStudent(
    long Id,
    long? UserId,
    string FirstName,
    string LastName,
    string Email,
    DateOnly? BirthDate,
    long SchoolId,
    string? SchoolName);

public record ApiStudentIn(
    string FirstName,
    string LastName,
    string Email,
    DateOnly? BirthDate,
    long SchoolId);

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

public record ApiEnrollmentIn(
    long StudentId,
    string AcademicYear,
    string? CourseName,
    string Status,
    DateTime? EnrolledAt,
    long SchoolId);

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

public record ApiAnnualFeeIn(
    long EnrollmentId,
    decimal Amount,
    string Currency,
    DateOnly DueDate,
    bool IsPaid,
    string? PaymentRef);
