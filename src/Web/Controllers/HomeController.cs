using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

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
        
        return View(scopes);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
