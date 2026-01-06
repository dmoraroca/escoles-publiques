namespace Domain.Entities;

/// <summary>
/// Entitat que representa un àmbit funcional dins el sistema.
/// </summary>
public class Scope
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
