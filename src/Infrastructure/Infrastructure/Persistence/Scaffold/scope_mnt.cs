using System;
using System.Collections.Generic;

namespace Infrastructure.Infrastructure.Persistence.Scaffold;

public partial class scope_mnt
{
    public long id { get; set; }

    public string name { get; set; } = null!;

    public string? description { get; set; }

    public bool is_active { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public virtual ICollection<school> schools { get; set; } = new List<school>();
}
