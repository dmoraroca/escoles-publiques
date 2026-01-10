using System;
using System.Collections.Generic;

namespace Infrastructure.Infrastructure.Persistence.Scaffold;

public partial class annual_fee
{
    public long id { get; set; }

    public long enrollment_id { get; set; }

    public decimal amount { get; set; }

    public string currency { get; set; } = null!;

    public DateOnly due_date { get; set; }

    public DateTime? paid_at { get; set; }

    public string? payment_ref { get; set; }

    public virtual enrollment enrollment { get; set; } = null!;
}
