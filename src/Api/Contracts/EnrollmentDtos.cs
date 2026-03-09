namespace Api.Contracts;
/// <summary>
/// Represents values and data structure for enrollment dto in.
/// </summary>
public record EnrollmentDtoIn(
    long StudentId,
    string AcademicYear,
    string? CourseName,
    string Status,
    DateTime? EnrolledAt,
    long SchoolId);
/// <summary>
/// Represents values and data structure for enrollment dto out.
/// </summary>
public record EnrollmentDtoOut(
    long Id,
    long StudentId,
    string StudentName,
    string AcademicYear,
    string? CourseName,
    string Status,
    DateTime EnrolledAt,
    long SchoolId,
    string SchoolName);
