namespace Domain.Entities;

public class School
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string? City { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
