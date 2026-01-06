using System;
using System.Collections.Generic;

namespace Infrastructure.Persistence.Scaffold;

public partial class Enrollment
{
    public long Id { get; set; }

    public long StudentId { get; set; }

    public string AcademicYear { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime EnrolledAt { get; set; }

    public string? CourseName { get; set; }

    public virtual ICollection<AnnualFee> AnnualFees { get; set; } = new List<AnnualFee>();

    public virtual Student Student { get; set; } = null!;
}
