using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;
namespace Web.Controllers;

/// <summary>
/// Controlador per gestionar la cerca global a la web.
/// </summary>
public class SearchController : Controller
{
    /// <summary>
    /// Mostra la pàgina principal de cerca.
    /// </summary>
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    /// <summary>
    /// Processa la cerca segons el query indicat.
    /// </summary>
    [HttpGet]
    public IActionResult Search(string query)
    {
        // TODO: Implementar cerca
        return RedirectToAction("Index", "Home");
    }
}
