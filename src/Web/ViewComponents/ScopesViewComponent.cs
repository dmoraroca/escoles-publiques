using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Web.Services.Api;

namespace Web.ViewComponents;
/// <summary>
/// Encapsulates the functional responsibility of scopes view component within the application architecture.
/// </summary>
public class ScopesViewComponent : ViewComponent
{
    private readonly IScopesApiClient _scopesApi;
    /// <summary>
    /// Initializes a new instance of the ScopesViewComponent class with its required dependencies.
    /// </summary>
    public ScopesViewComponent(IScopesApiClient scopesApi)
    {
        _scopesApi = scopesApi;
    }
    /// <summary>
    /// Executes middleware logic for the current HTTP request.
    /// </summary>
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var scopes = await _scopesApi.GetAllAsync();
        var scopeViewModels = scopes.Select(s => new ScopeViewModel
        {
            Name = s.Name?.Trim() ?? string.Empty,
            Url = "#" // Pots canviar per una URL real si cal
        }).ToList();

        return View(scopeViewModels);
    }
}
