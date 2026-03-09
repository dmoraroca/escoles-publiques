using System;
using System.Collections.Generic;

namespace Infrastructure.Persistence.Scaffold;
/// <summary>
/// Encapsulates the functional responsibility of scope mnt within the application architecture.
/// </summary>
public partial class ScopeMnt
{
            public int Id { get; set; }

            public string Name { get; set; } = null!;

            public string? Description { get; set; }

            public bool IsActive { get; set; }

            public DateTime CreatedAt { get; set; }

            public DateTime UpdatedAt { get; set; }
}
