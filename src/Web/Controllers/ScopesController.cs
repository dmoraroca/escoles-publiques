using Microsoft.AspNetCore.Mvc;
using Web.Models;

using Microsoft.AspNetCore.Authorization;
namespace Web.Controllers;

/// <summary>
/// Controlador per mostrar els diferents àmbits del projecte.
/// </summary>
public class ScopesController : Controller
{
    /// <summary>
    /// Mostra la llista d'àmbits disponibles com a partial view.
    /// </summary>
    public IActionResult Index()
    {
        var scopes = new List<ScopeViewModel>
        {
            new ScopeViewModel { Name = "Centres i serveis educatius", Url = "#" },
            new ScopeViewModel { Name = "Professorat", Url = "#" },
            new ScopeViewModel { Name = "Famílies", Url = "#" },
            new ScopeViewModel { Name = "Tria educativa", Url = "#" },
            new ScopeViewModel { Name = "Catalunya 2030", Url = "#" },
            new ScopeViewModel { Name = "Mapa escolar", Url = "#" },
            new ScopeViewModel { Name = "Homologació d'estudis estrangers", Url = "#" },
            new ScopeViewModel { Name = "Portal de centre", Url = "#" }
        };
        
        return PartialView("_Scopes", scopes);
    }
}
