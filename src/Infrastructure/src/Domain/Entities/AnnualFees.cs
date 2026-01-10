using System;
using System.Collections.Generic;

namespace Infrastructure.src.Domain.Entities;

public partial class AnnualFees
{
    public long Id { get; set; }

    public long EnrollmentId { get; set; }

    public decimal Amount { get; set; }

    public string Currency { get; set; } = null!;

    public DateOnly DueDate { get; set; }

    public DateTime? PaidAt { get; set; }

    public string? PaymentRef { get; set; }

    public long? StudentId { get; set; }

    public virtual Enrollments Enrollment { get; set; } = null!;

    public virtual Students? Student { get; set; }
}
