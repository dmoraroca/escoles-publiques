using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.ViewComponents;

public class ScopesViewComponent : ViewComponent
{
    private readonly IScopeRepository _scopeRepository;

    public ScopesViewComponent(IScopeRepository scopeRepository)
    {
        _scopeRepository = scopeRepository;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var scopes = await _scopeRepository.GetAllActiveScopesAsync();
        var scopeViewModels = scopes.Select(s => new ScopeViewModel
        {
            Name = s.Name,
            Url = "#" // Pots canviar per una URL real si cal
        }).ToList();
        
        return View(scopeViewModels);
    }
}
