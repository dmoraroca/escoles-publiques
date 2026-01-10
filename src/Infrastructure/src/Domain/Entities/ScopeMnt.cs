using System;
using System.Collections.Generic;

namespace Infrastructure.src.Domain.Entities;

/// <summary>
/// Taula de manteniment dels àmbits/nivells educatius de les escoles
/// </summary>
public partial class ScopeMnt
{
    /// <summary>
    /// Identificador únic de l&apos;àmbit
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nom de l&apos;àmbit educatiu
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Descripció de l&apos;àmbit
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Indica si l&apos;àmbit està actiu
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Data de creació del registre
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Data d&apos;última actualització
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
