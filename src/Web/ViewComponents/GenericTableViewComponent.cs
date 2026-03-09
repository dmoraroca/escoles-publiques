using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.ViewComponents;
/// <summary>
/// Encapsulates the functional responsibility of generic table view component within the application architecture.
/// </summary>
public class GenericTableViewComponent : ViewComponent
{
    /// <summary>
    /// Executes middleware logic for the current HTTP request.
    /// </summary>
    public IViewComponentResult Invoke(object model)
    {
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model), "El model no pot ser null");
        }

        // El model ha de ser un TableViewModel<T>
        var modelType = model.GetType();
        if (!modelType.IsGenericType || modelType.GetGenericTypeDefinition() != typeof(TableViewModel<>))
        {
            throw new ArgumentException("El model ha de ser de tipus TableViewModel<T>", nameof(model));
        }

        return View("Default", model);
    }
}
