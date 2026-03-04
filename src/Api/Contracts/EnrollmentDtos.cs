namespace Api.Contracts;

public record EnrollmentDtoIn(
    long StudentId,
    string AcademicYear,
    string? CourseName,
    string Status,
    DateTime? EnrolledAt,
    long SchoolId);

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
