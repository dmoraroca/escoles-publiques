namespace Domain.Entities;
/// <summary>
/// Encapsulates the functional responsibility of enrollment within the application architecture.
/// </summary>
public class Enrollment
{
    public long Id { get; set; }
    public long StudentId { get; set; }
    public string AcademicYear { get; set; } = null!;
    public string? CourseName { get; set; }
    public string Status { get; set; } = null!;
    public DateTime EnrolledAt { get; set; }
    public long SchoolId { get; set; }

    public virtual Student Student { get; set; } = null!;
    public virtual School? School { get; set; }
    public virtual ICollection<AnnualFee> AnnualFees { get; set; } = new List<AnnualFee>();
}
