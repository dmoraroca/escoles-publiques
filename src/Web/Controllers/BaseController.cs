using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;
namespace Web.Controllers;
/// <summary>
/// Exposes HTTP endpoints to manage base workflows.
/// </summary>
public abstract class BaseController : Controller
{
    protected readonly ILogger Logger;
    protected readonly IStringLocalizer Localizer;
            /// <summary>
            /// Initializes a new instance of the BaseController class with its required dependencies.
            /// </summary>
            protected BaseController(ILogger logger, IStringLocalizer? localizer = null)
    {
        Logger = logger;
        Localizer = localizer ?? new PassthroughStringLocalizer();
    }
            /// <summary>
            /// Handles error and executes the corresponding use case.
            /// </summary>
            protected IActionResult HandleError(Exception ex, string action)
    {
        Logger.LogError(ex, "Error al executar {Action}", action);
        TempData["Error"] = Localizer["Hi ha hagut un error. Si us plau, torna-ho a intentar."].Value;
        return RedirectToAction("Error", "Home");
    }
            /// <summary>
            /// Executes the set success message operation as part of this component.
            /// </summary>
            protected void SetSuccessMessage(string message)
    {
        TempData["Success"] = message;
    }
            /// <summary>
            /// Executes the set error message operation as part of this component.
            /// </summary>
            protected void SetErrorMessage(string message)
    {
        TempData["Error"] = message;
    }
            /// <summary>
            /// Executes the is unauthorized operation as part of this component.
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
            /// <summary>
            /// Executes the is ajax request operation as part of this component.
            /// </summary>
            protected bool IsAjaxRequest()
    {
        if (Request?.Headers == null) return false;
        if (Request.Headers.TryGetValue("X-Requested-With", out var value)
            && value == "XMLHttpRequest")
        {
            return true;
        }
        var accept = Request.Headers["Accept"].ToString();
        return accept.Contains("application/json", StringComparison.OrdinalIgnoreCase);
    }
            /// <summary>
            /// Encapsulates the functional responsibility of passthrough string localizer within the application architecture.
            /// </summary>
            private sealed class PassthroughStringLocalizer : IStringLocalizer
    {
        public LocalizedString this[string name] => new(name, name, resourceNotFound: true);

        public LocalizedString this[string name, params object[] arguments]
            => new(name, string.Format(CultureInfo.CurrentCulture, name, arguments), resourceNotFound: true);
                        /// <summary>
                        /// Retrieves all strings and returns it to the caller.
                        /// </summary>
                        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
            => Enumerable.Empty<LocalizedString>();
    }
}
