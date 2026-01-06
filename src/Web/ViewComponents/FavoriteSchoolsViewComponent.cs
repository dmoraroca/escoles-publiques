using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.ViewComponents;

/// <summary>
/// ViewComponent for displaying a user's favorite schools in the UI.
/// </summary>
public class FavoriteSchoolsViewComponent : ViewComponent
{
    private readonly ISchoolService _schoolService;

    public FavoriteSchoolsViewComponent(ISchoolService schoolService)
    {
        _schoolService = schoolService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var schools = await _schoolService.GetAllSchoolsAsync();
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
