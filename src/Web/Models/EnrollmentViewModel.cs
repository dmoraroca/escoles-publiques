namespace Web.Models;

public class EnrollmentViewModel
{
    public int Id { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string AcademicYear { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime EnrolledAt { get; set; }
}
