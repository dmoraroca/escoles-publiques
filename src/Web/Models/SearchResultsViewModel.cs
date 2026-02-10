namespace Web.Models;

/// <summary>
/// Model de vista per mostrar els resultats d'una cerca global.
/// </summary>
public class SearchResultsViewModel
{
    public string? SearchQuery { get; set; }
    public string? ScopeName { get; set; }
    public List<SchoolResultViewModel> Schools { get; set; } = new();
    public List<StudentResultViewModel> Students { get; set; } = new();
    public List<EnrollmentResultViewModel> Enrollments { get; set; } = new();
    public List<AnnualFeeResultViewModel> AnnualFees { get; set; } = new();

    public bool HasResults => Schools.Any() || Students.Any() || Enrollments.Any() || AnnualFees.Any();
}

/// <summary>
/// Resultat d'escola en la cerca.
/// </summary>
public class SchoolResultViewModel
{
    // Per compatibilitat amb vistes antigues
    public string? Scope { get; set; }
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? City { get; set; }
    public long? ScopeId { get; set; }
    public string? ScopeName { get; set; }
}

/// <summary>
/// Resultat d'alumne en la cerca.
/// </summary>
public class StudentResultViewModel
{
    public long Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? SchoolName { get; set; }
}

/// <summary>
/// Resultat d'inscripció en la cerca.
/// </summary>
public class EnrollmentResultViewModel
{
    public long Id { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string SchoolName { get; set; } = string.Empty;
    public string AcademicYear { get; set; } = string.Empty;
    public DateTime EnrollmentDate { get; set; }
}

public class AnnualFeeResultViewModel
{
    public long Id { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "EUR";
    public DateOnly DueDate { get; set; }
    public bool IsPaid { get; set; }
}
