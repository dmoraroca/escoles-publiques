using System;
using System.Collections.Generic;

namespace Infrastructure.src.Infrastructure.Persistence.Scaffold;

public partial class School
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public string? City { get; set; }

    public bool IsFavorite { get; set; }

    public DateTime CreatedAt { get; set; }

    public long? ScopeId { get; set; }

    public virtual ScopeMnt? Scope { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
