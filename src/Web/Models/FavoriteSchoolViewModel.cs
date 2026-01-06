namespace Web.Models;

/// <summary>
/// Model de vista per mostrar una escola favorita.
/// </summary>
public class FavoriteSchoolViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Municipality { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
