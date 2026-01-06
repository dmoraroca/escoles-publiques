namespace Web.Models;

/// <summary>
/// Model de vista per gestionar les dades d'un alumne.
/// </summary>
public class StudentViewModel
{
    public int Id { get; set; }
    public long? UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; }
    public int SchoolId { get; set; }
    public string SchoolName { get; set; } = string.Empty;
}
