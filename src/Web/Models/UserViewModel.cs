namespace Web.Models;
/// <summary>
/// Encapsulates the functional responsibility of user view model within the application architecture.
/// </summary>
public class UserViewModel
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; }
    public int? StudentId { get; set; }
}
