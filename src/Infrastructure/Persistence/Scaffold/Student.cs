using System;
using System.Collections.Generic;

namespace Infrastructure.Persistence.Scaffold;
/// <summary>
/// Encapsulates the functional responsibility of student within the application architecture.
/// </summary>
public partial class Student
{
    public long Id { get; set; }

    public long SchoolId { get; set; }

    public DateTime CreatedAt { get; set; }

    public long? UserId { get; set; }

    public virtual ICollection<AnnualFee> AnnualFees { get; set; } = new List<AnnualFee>();

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public virtual School School { get; set; } = null!;

    public virtual User? User { get; set; }
}
