using System;
using System.Collections.Generic;

namespace Infrastructure.src.Domain.Entities;

public partial class Students
{
    public long Id { get; set; }

    public long SchoolId { get; set; }

    public DateTime CreatedAt { get; set; }

    public long? UserId { get; set; }

    public virtual ICollection<AnnualFees> AnnualFees { get; set; } = new List<AnnualFees>();

    public virtual ICollection<Enrollments> Enrollments { get; set; } = new List<Enrollments>();

    public virtual Schools School { get; set; } = null!;

    public virtual Users? User { get; set; }
}
