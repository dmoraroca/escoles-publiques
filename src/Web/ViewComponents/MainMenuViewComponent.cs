using Microsoft.AspNetCore.Mvc;

namespace Web.ViewComponents;
/// <summary>
/// Encapsulates the functional responsibility of main menu view component within the application architecture.
/// </summary>
public class MainMenuViewComponent : ViewComponent
{
            /// <summary>
            /// Executes middleware logic for the current HTTP request.
            /// </summary>
            public IViewComponentResult Invoke()
    {
        var currentController = ViewContext.RouteData.Values["controller"]?.ToString();
        ViewBag.CurrentController = currentController;

        return View();
    }
}
