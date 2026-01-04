namespace Web.Models;

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
}
