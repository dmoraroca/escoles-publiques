using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;
namespace Web.Controllers;

public abstract class BaseController : Controller
{
    protected readonly ILogger Logger;

    protected BaseController(ILogger logger)
    {
        Logger = logger;
    }

    protected IActionResult HandleError(Exception ex, string action)
    {
        Logger.LogError(ex, "Error al executar {Action}", action);
        TempData["Error"] = "Hi ha hagut un error. Si us plau, torna-ho a intentar.";
        return RedirectToAction("Error", "Home");
    }

    protected void SetSuccessMessage(string message)
    {
        TempData["Success"] = message;
    }

    protected void SetErrorMessage(string message)
    {
        TempData["Error"] = message;
    }
}
