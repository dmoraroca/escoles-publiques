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

    /// <summary>
    /// Indica si l&apos;escola és marcada com a favorita
    /// </summary>
    public bool IsFavorite { get; set; }

    /// <summary>
    /// Àmbit de l&apos;escola (ex: Infantil, Primària, Secundària, Batxillerat)
    /// </summary>
    public string? Scope { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
