using System;
using System.Collections.Generic;

namespace Infrastructure.src.Domain.Entities;

public partial class Enrollments
{
    public long Id { get; set; }

    public long StudentId { get; set; }

    public string AcademicYear { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime EnrolledAt { get; set; }

    public string? CourseName { get; set; }

    public virtual ICollection<AnnualFees> AnnualFees { get; set; } = new List<AnnualFees>();

    public virtual Students Student { get; set; } = null!;
}
