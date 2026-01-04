using Microsoft.AspNetCore.Mvc;

namespace Web.ViewComponents;

public class MainMenuViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var currentController = ViewContext.RouteData.Values["controller"]?.ToString();
        ViewBag.CurrentController = currentController;
        
        return View();
    }
}
