using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;
namespace Web.Controllers;

public class SearchController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Search(string query)
    {
        // TODO: Implementar cerca
        return RedirectToAction("Index", "Home");
    }
}
