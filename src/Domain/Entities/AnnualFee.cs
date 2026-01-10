using Domain.Entities;
namespace Domain.Entities;

/// <summary>
/// Entitat que representa una quota anual associada a una inscripció.
/// </summary>
public class AnnualFee
{
    public long Id { get; set; }
    public long EnrollmentId { get; set; }
    // Eliminat: public long StudentId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = null!;
    public DateOnly DueDate { get; set; }
    public DateTime? PaidAt { get; set; }
    public string? PaymentRef { get; set; }

    public virtual Enrollment Enrollment { get; set; } = null!;
    // Eliminat: public virtual Student Student { get; set; } = null!;
}
