using System;
using System.Collections.Generic;

namespace Infrastructure.src.Infrastructure.Persistence.Scaffold;

public partial class ScopeMnt
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<School> Schools { get; set; } = new List<School>();
}
