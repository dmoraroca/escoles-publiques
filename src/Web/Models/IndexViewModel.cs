namespace Web.Models;

public class IndexViewModel
{
    public List<FavoriteSchoolViewModel> FavoriteSchools { get; set; } = new();
    public List<ScopeViewModel> Scopes { get; set; } = new();
}
