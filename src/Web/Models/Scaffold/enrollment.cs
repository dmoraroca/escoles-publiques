using System;
using System.Collections.Generic;

namespace Web.Models.Scaffold;

public partial class enrollment
{
    public long id { get; set; }

    public long student_id { get; set; }

    public string academic_year { get; set; } = null!;

    public string? course_name { get; set; }

    public string status { get; set; } = null!;

    public DateTime enrolled_at { get; set; }

    public virtual ICollection<annual_fee> annual_fees { get; set; } = new List<annual_fee>();

    public virtual student student { get; set; } = null!;
}
