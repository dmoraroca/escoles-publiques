namespace Web.Models;

/// <summary>
/// Model de vista per gestionar les quotes anuals d'una inscripció.
/// </summary>
public class AnnualFeeViewModel
{
    public int SchoolId { get; set; }
    public long Id { get; set; }
    public int EnrollmentId { get; set; }
    public string EnrollmentInfo { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "EUR";
    public DateOnly DueDate { get; set; }
    public string PaymentStatus { get; set; } = "Pending";
    public bool IsPaid { get; set; }
    public DateTime? PaidAt { get; set; }
    public string? PaymentRef { get; set; }
    public DateTime CreatedAt { get; set; }

    public string AcademicYear { get; set; } = string.Empty;
    public string SchoolName { get; set; } = string.Empty;
}
