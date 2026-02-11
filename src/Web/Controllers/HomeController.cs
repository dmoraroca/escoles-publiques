using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Models;
using System.Security.Claims;
using Microsoft.Extensions.Localization;

namespace Web.Controllers;

/// <summary>
/// Controlador principal de la web. Gestiona la pàgina d'inici, privacitat i errors.
/// </summary>
/// <summary>
/// Controlador principal de la web. Gestiona la pàgina d'inici, privacitat i errors.
/// </summary>
[Authorize]
public partial class HomeController : BaseController
{
    /// <summary>
    /// Constructor del controlador Home.
    /// </summary>
    /// <summary>
    /// Constructor del controlador Home.
    /// </summary>
    public HomeController(ILogger<HomeController> logger, IStringLocalizer<BaseController> localizer) : base(logger, localizer)
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
        ViewBag.SearchQuery = searchQuery?.Trim();
        ViewBag.ScopeName = scopeName?.Trim();
        var role = User.FindFirstValue("Role");
        ViewBag.UserRole = role;

        if (role == "USER")
        {
            // Redirigeix directament al dashboard d'usuari
            return RedirectToAction("Dashboard", "Dashboard");
        }

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
