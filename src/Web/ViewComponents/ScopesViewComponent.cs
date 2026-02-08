using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Web.Services.Api;

namespace Web.ViewComponents;

/// <summary>
/// ViewComponent for displaying available scopes in the UI.
/// </summary>
public class ScopesViewComponent : ViewComponent
{
    private readonly IScopesApiClient _scopesApi;

    public ScopesViewComponent(IScopesApiClient scopesApi)
    {
        _scopesApi = scopesApi;
    }

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
