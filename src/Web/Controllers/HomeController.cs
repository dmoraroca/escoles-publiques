using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Models;
using System.Security.Claims;
using Microsoft.Extensions.Localization;

namespace Web.Controllers;

/// <summary>
/// Exposes HTTP endpoints to manage home workflows.
/// </summary>
[Authorize]
public partial class HomeController : BaseController
{
    /// <summary>
    /// Initializes a new instance of the HomeController class with its required dependencies.
    /// </summary>
    public HomeController(ILogger<HomeController> logger, IStringLocalizer<BaseController>? localizer = null) : base(logger, localizer)
    {
    }
    /// <summary>
    /// Executes the index operation as part of this component.
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
    /// Executes the privacy operation as part of this component.
    /// </summary>
    public IActionResult Privacy()
    {
        return View();
    }
    /// <summary>
    /// Executes the error operation as part of this component.
    /// </summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
