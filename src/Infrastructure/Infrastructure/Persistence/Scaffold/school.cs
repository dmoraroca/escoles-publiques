using System;
using System.Collections.Generic;

namespace Infrastructure.Infrastructure.Persistence.Scaffold;

public partial class school
{
    public long id { get; set; }

    public string name { get; set; } = null!;

    public string code { get; set; } = null!;

    public string? city { get; set; }

    public bool is_favorite { get; set; }

    public DateTime created_at { get; set; }

    public long? scope_id { get; set; }

    public virtual scope_mnt? scope { get; set; }

    public virtual ICollection<student> students { get; set; } = new List<student>();
}
