using System;
using System.Collections.Generic;

namespace Infrastructure.src.Domain.Entities;

public partial class Users
{
    public long Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Role { get; set; } = null!;

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public DateOnly? BirthDate { get; set; }

    public virtual ICollection<Students> Students { get; set; } = new List<Students>();
}
