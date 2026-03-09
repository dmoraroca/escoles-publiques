using System;
using System.Collections.Generic;

namespace Web.Models.Scaffold;
/// <summary>
/// Encapsulates the functional responsibility of scope mnt within the application architecture.
/// </summary>
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
