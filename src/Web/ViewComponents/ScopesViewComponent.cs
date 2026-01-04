using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.ViewComponents;

public class ScopesViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var scopes = new List<ScopeViewModel>
        {
            new ScopeViewModel { Name = "Escoles públiques", Url = "#" },
            new ScopeViewModel { Name = "Escoles privades", Url = "#" },
            new ScopeViewModel { Name = "Formació professional", Url = "#" }
        };
        
        return View(scopes);
    }
}
