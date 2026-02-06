using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;
namespace Web.Controllers;

/// <summary>
/// Classe base per als controladors, amb gestió d'errors i missatges.
/// </summary>
public abstract class BaseController : Controller
{
    protected readonly ILogger Logger;

    /// <summary>
    /// Constructor de la classe base amb logger.
    /// </summary>
    protected BaseController(ILogger logger)
    {
        Logger = logger;
    }

    /// <summary>
    /// Gestiona errors i redirigeix a la pàgina d'error.
    /// </summary>
    protected IActionResult HandleError(Exception ex, string action)
    {
        Logger.LogError(ex, "Error al executar {Action}", action);
        TempData["Error"] = "Hi ha hagut un error. Si us plau, torna-ho a intentar.";
        return RedirectToAction("Error", "Home");
    }

    /// <summary>
    /// Assigna un missatge d'èxit a TempData.
    /// </summary>
    protected void SetSuccessMessage(string message)
    {
        TempData["Success"] = message;
    }

    /// <summary>
    /// Assigna un missatge d'error a TempData.
    /// </summary>
    protected void SetErrorMessage(string message)
    {
        TempData["Error"] = message;
    }

    /// <summary>
    /// Indica si una excepció és per accés no autoritzat.
    /// </summary>
    protected static bool IsUnauthorized(Exception ex)
    {
        if (ex is HttpRequestException httpEx && httpEx.StatusCode.HasValue)
        {
            return httpEx.StatusCode == System.Net.HttpStatusCode.Unauthorized
                || httpEx.StatusCode == System.Net.HttpStatusCode.Forbidden;
        }

        return false;
    }
}
