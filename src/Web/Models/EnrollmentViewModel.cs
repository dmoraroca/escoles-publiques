namespace Web.Models;
/// <summary>
/// Encapsulates the functional responsibility of enrollment view model within the application architecture.
/// </summary>
public class EnrollmentViewModel
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int Year { get; set; }
    public string AcademicYear { get; set; } = string.Empty;
    public string? CourseName { get; set; }
    public string? EnrollmentType { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime EnrolledAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public int SchoolId { get; set; }
    public string SchoolName { get; set; } = string.Empty;
    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Cal acceptar la política de privacitat.")]
    public bool AgreesToPrivacy { get; set; } = false;
}
