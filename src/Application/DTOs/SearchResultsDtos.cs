namespace Application.DTOs;
/// <summary>
/// Encapsulates the functional responsibility of search results dto within the application architecture.
/// </summary>
public class SearchResultsDto
{
    public string? SearchQuery { get; set; }
    public string? ScopeName { get; set; }
    public List<SchoolResultDto> Schools { get; set; } = new();
    public List<StudentResultDto> Students { get; set; } = new();
    public List<EnrollmentResultDto> Enrollments { get; set; } = new();
    public List<AnnualFeeResultDto> AnnualFees { get; set; } = new();

    public bool HasResults => Schools.Any() || Students.Any() || Enrollments.Any() || AnnualFees.Any();
}
/// <summary>
/// Encapsulates the functional responsibility of school result dto within the application architecture.
/// </summary>
public class SchoolResultDto
{
    public string? Scope { get; set; }
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? City { get; set; }
    public long? ScopeId { get; set; }
    public string? ScopeName { get; set; }
}
/// <summary>
/// Encapsulates the functional responsibility of student result dto within the application architecture.
/// </summary>
public class StudentResultDto
{
    public long Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? SchoolName { get; set; }
}
/// <summary>
/// Encapsulates the functional responsibility of enrollment result dto within the application architecture.
/// </summary>
public class EnrollmentResultDto
{
    public long Id { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string SchoolName { get; set; } = string.Empty;
    public string AcademicYear { get; set; } = string.Empty;
    public DateTime EnrollmentDate { get; set; }
}
/// <summary>
/// Encapsulates the functional responsibility of annual fee result dto within the application architecture.
/// </summary>
public class AnnualFeeResultDto
{
    public long Id { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "EUR";
    public DateOnly DueDate { get; set; }
    public bool IsPaid { get; set; }
}
