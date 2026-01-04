using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.ViewComponents;

/// <summary>
/// ViewComponent genèric per renderitzar taules amb dades dinàmiques
/// Suporta: ordenació, cerca, paginació, accions personalitzades, responsive
/// </summary>
public class GenericTableViewComponent : ViewComponent
{
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
