namespace Web.Models;
/// <summary>
/// Encapsulates the functional responsibility of index view model within the application architecture.
/// </summary>
public class IndexViewModel
{
    public List<FavoriteSchoolViewModel> FavoriteSchools { get; set; } = new();
    public List<ScopeViewModel> Scopes { get; set; } = new();
}
