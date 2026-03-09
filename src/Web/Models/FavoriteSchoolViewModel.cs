namespace Web.Models;
/// <summary>
/// Encapsulates the functional responsibility of favorite school view model within the application architecture.
/// </summary>
public class FavoriteSchoolViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Municipality { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
