using Microsoft.AspNetCore.Mvc;

namespace Web.ViewComponents;

/// <summary>
/// ViewComponent for rendering the main navigation menu in the application.
/// </summary>
public class MainMenuViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var currentController = ViewContext.RouteData.Values["controller"]?.ToString();
        ViewBag.CurrentController = currentController;

        return View();
    }
}
