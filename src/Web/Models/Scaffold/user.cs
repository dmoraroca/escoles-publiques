using System;
using System.Collections.Generic;

namespace Web.Models.Scaffold;

public partial class user
{
    public long id { get; set; }

    public string first_name { get; set; } = null!;

    public string last_name { get; set; } = null!;

    public string email { get; set; } = null!;

    public string password_hash { get; set; } = null!;

    public string role { get; set; } = null!;

    public DateOnly? birth_date { get; set; }

    public bool is_active { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public DateTime? last_login_at { get; set; }

    public virtual student? student { get; set; }
}
