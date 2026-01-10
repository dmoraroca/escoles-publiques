using System;
using System.Collections.Generic;

namespace Infrastructure.src.Infrastructure.Persistence.Scaffold;

public partial class Student
{
    public long Id { get; set; }

    public long SchoolId { get; set; }

    public long? UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public virtual School School { get; set; } = null!;

    public virtual User? User { get; set; }
}
