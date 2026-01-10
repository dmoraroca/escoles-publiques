namespace Domain.Entities;

/// <summary>
/// Entitat que representa una escola dins el domini.
/// </summary>
public class School
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string? City { get; set; }
    public bool IsFavorite { get; set; }
    public long? ScopeId { get; set; }
    public virtual Scope? Scope { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
