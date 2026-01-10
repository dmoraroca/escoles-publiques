using System;
using System.Collections.Generic;

namespace Infrastructure.Persistence.Scaffold;

public partial class School
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public string? City { get; set; }

    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Indica si l&apos;escola és marcada com a favorita
    /// </summary>
    public bool IsFavorite { get; set; }

    public long? ScopeId { get; set; }

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
