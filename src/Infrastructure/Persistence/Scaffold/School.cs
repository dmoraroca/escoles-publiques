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

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
