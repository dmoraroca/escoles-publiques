namespace Web.Models;

public class AnnualFeeViewModel
{
    public int Id { get; set; }
    public int EnrollmentId { get; set; }
    public string EnrollmentInfo { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "EUR";
    public DateTime DueDate { get; set; }
    public string PaymentStatus { get; set; } = "Pending";
    public bool IsPaid { get; set; }
    public DateTime? PaidAt { get; set; }
    public string? PaymentRef { get; set; }
    public DateTime CreatedAt { get; set; }
}
