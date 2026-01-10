using System;
using System.Collections.Generic;

namespace Web.Models.Scaffold;

public partial class student
{
    public long id { get; set; }

    public long school_id { get; set; }

    public long? user_id { get; set; }

    public DateTime created_at { get; set; }

    public virtual ICollection<enrollment> enrollments { get; set; } = new List<enrollment>();

    public virtual school school { get; set; } = null!;

    public virtual user? user { get; set; }
}
