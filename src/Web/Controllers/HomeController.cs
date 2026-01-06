using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers;

/// <summary>
/// Controlador principal de la web. Gestiona la pàgina d'inici, privacitat i errors.
/// </summary>
/// <summary>
/// Controlador principal de la web. Gestiona la pàgina d'inici, privacitat i errors.
/// </summary>
[Authorize]
public class HomeController : BaseController
{
    /// <summary>
    /// Constructor del controlador Home.
    /// </summary>
    /// <summary>
    /// Constructor del controlador Home.
    /// </summary>
    public HomeController(ILogger<HomeController> logger) : base(logger)
    {
    }

    /// <summary>
    /// Mostra la pàgina principal amb opcions de cerca i filtratge per àmbit.
    /// </summary>
    /// <summary>
    /// Mostra la pàgina principal amb opcions de cerca i filtratge per àmbit.
    /// </summary>
    public IActionResult Index(string? searchQuery, string? scopeName)
    {
        ViewBag.SearchQuery = searchQuery;
        ViewBag.ScopeName = scopeName;
        return View();
    }

    /// <summary>
    /// Mostra la pàgina de privacitat.
    /// </summary>
    /// <summary>
    /// Mostra la pàgina de privacitat.
    /// </summary>
    public IActionResult Privacy()
    {
        return View();
    }

    /// <summary>
    /// Mostra la pàgina d'error amb informació de la petició.
    /// </summary>
    /// <summary>
    /// Mostra la pàgina d'error amb informació de la petició.
    /// </summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
