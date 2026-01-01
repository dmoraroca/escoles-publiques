using System;
using System.Collections.Generic;

namespace Infrastructure.Persistence.Scaffold;

public partial class Student
{
    public long Id { get; set; }

    public long SchoolId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateOnly? BirthDate { get; set; }

    public string? Email { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public virtual School School { get; set; } = null!;
}
