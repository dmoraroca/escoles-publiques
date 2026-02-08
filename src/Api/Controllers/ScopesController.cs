using Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ScopesController : ControllerBase
{
    private readonly IScopeRepository _scopeRepository;

    public ScopesController(IScopeRepository scopeRepository)
    {
        _scopeRepository = scopeRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var scopes = await _scopeRepository.GetAllActiveScopesAsync();
        var result = scopes.Select(s => new ScopeDtoOut(s.Id, s.Name));
        return Ok(result);
    }
}

public record ScopeDtoOut(long Id, string Name);
