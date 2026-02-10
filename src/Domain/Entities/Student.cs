using Domain.Entities;
namespace Domain.Entities;

/// <summary>
/// Entitat que representa un alumne dins el domini.
/// </summary>
public class Student
{
    public long Id { get; set; }
    public long SchoolId { get; set; }
    public long? UserId { get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual School School { get; set; } = null!;
    public virtual User? User { get; set; }
    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
