using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers;

public class HomeController : BaseController
{
    public HomeController(ILogger<HomeController> logger) : base(logger)
    {
    }

    public IActionResult Index(string? searchQuery, string? scopeName)
    {
        ViewBag.SearchQuery = searchQuery;
        ViewBag.ScopeName = scopeName;
        return View();
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
