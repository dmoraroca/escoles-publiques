using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Web.Services.Api;

namespace Web.ViewComponents;

/// <summary>
/// ViewComponent for displaying a user's favorite schools in the UI.
/// </summary>
public class FavoriteSchoolsViewComponent : ViewComponent
{
    private readonly ISchoolsApiClient _schoolApi;

    public FavoriteSchoolsViewComponent(ISchoolsApiClient schoolApi)
    {
        _schoolApi = schoolApi;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var schools = await _schoolApi.GetAllAsync();
        var favoriteSchools = schools
            .Where(s => s.IsFavorite)
            .Take(10)
            .Select(s => new FavoriteSchoolViewModel
            {
                Id = (int)s.Id,
                Name = s.Name,
                Municipality = s.City ?? "Sense ciutat",
                Url = Url.Action("Details", "Schools", new { id = s.Id })
            })
            .ToList();

        return View(favoriteSchools);
    }
}
