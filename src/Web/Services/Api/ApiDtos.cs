namespace Web.Services.Api;

public record ApiScope(long Id, string Name);

public record ApiStudent(long Id, string FirstName, string LastName, string Email, string? SchoolName);

public record ApiEnrollment(long Id, string StudentName, string SchoolName, string AcademicYear, DateTime EnrolledAt);

public record ApiAnnualFee(long Id, string StudentName, decimal Amount, string Currency, DateOnly DueDate, bool IsPaid);
