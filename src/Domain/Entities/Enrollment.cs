namespace Domain.Entities;

public class Enrollment
{
    public long Id { get; set; }
    public long StudentId { get; set; }
    public string AcademicYear { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime EnrolledAt { get; set; }
    
    public virtual Student Student { get; set; } = null!;
    public virtual ICollection<AnnualFee> AnnualFees { get; set; } = new List<AnnualFee>();
}
