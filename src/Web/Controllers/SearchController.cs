using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;
namespace Web.Controllers;
/// <summary>
/// Exposes HTTP endpoints to manage search workflows.
/// </summary>
public class SearchController : Controller
{
    /// <summary>
    /// Executes the index operation as part of this component.
    /// </summary>
    public IActionResult Index()
    {
        return View();
    }
    /// <summary>
    /// Executes the search operation as part of this component.
    /// </summary>
    [HttpGet]
    public IActionResult Search(string query)
    {
        return RedirectToAction("Index", "Home");
    }
}
