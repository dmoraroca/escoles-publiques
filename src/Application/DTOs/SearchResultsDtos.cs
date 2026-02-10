namespace Application.DTOs;

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

public class StudentResultDto
{
    public long Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? SchoolName { get; set; }
}

public class EnrollmentResultDto
{
    public long Id { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string SchoolName { get; set; } = string.Empty;
    public string AcademicYear { get; set; } = string.Empty;
    public DateTime EnrollmentDate { get; set; }
}

public class AnnualFeeResultDto
{
    public long Id { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "EUR";
    public DateOnly DueDate { get; set; }
    public bool IsPaid { get; set; }
}
