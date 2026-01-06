using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.ViewComponents;

/// <summary>
/// ViewComponent for rendering generic tables with dynamic data. Supports sorting, searching, pagination, custom actions, and responsive design.
/// </summary>
public class GenericTableViewComponent : ViewComponent
{
    /// <summary>
    /// Invokes the view component to render a table using the provided model.
    /// </summary>
    /// <param name="model">A TableViewModel&lt;T&gt; instance containing table data.</param>
    /// <returns>The rendered table view.</returns>
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
