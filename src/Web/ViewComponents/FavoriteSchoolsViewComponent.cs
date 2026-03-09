using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Web.Services.Api;

namespace Web.ViewComponents;
/// <summary>
/// Encapsulates the functional responsibility of favorite schools view component within the application architecture.
/// </summary>
public class FavoriteSchoolsViewComponent : ViewComponent
{
    private readonly ISchoolsApiClient _schoolApi;
    /// <summary>
    /// Initializes a new instance of the FavoriteSchoolsViewComponent class with its required dependencies.
    /// </summary>
    public FavoriteSchoolsViewComponent(ISchoolsApiClient schoolApi)
    {
        _schoolApi = schoolApi;
    }
    /// <summary>
    /// Executes middleware logic for the current HTTP request.
    /// </summary>
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
                Url = Url.Action("Details", "Schools", new { id = s.Id }) ?? string.Empty
            })
            .ToList();

        return View(favoriteSchools);
    }
}
