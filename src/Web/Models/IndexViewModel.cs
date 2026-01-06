namespace Web.Models;

/// <summary>
/// Model de vista per la pàgina principal (escoles favorites i àmbits).
/// </summary>
public class IndexViewModel
{
    public List<FavoriteSchoolViewModel> FavoriteSchools { get; set; } = new();
    public List<ScopeViewModel> Scopes { get; set; } = new();
}
