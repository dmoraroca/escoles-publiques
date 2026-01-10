using System;
using System.Collections.Generic;

namespace Infrastructure.src.Infrastructure.Persistence.Scaffold;

public partial class AnnualFee
{
    public long Id { get; set; }

    public long EnrollmentId { get; set; }

    public decimal Amount { get; set; }

    public string Currency { get; set; } = null!;

    public DateOnly DueDate { get; set; }

    public DateTime? PaidAt { get; set; }

    public string? PaymentRef { get; set; }

    public virtual Enrollment Enrollment { get; set; } = null!;
}
