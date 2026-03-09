using Api.Contracts;
using Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Exposes HTTP endpoints to manage scopes workflows.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ScopesController : ControllerBase
{
    private readonly IScopeRepository _scopeRepository;
            /// <summary>
            /// Initializes a new instance of the ScopesController class with its required dependencies.
            /// </summary>
            public ScopesController(IScopeRepository scopeRepository)
    {
        _scopeRepository = scopeRepository;
    }
    /// <summary>
    /// Retrieves all and returns it to the caller.
    /// </summary>
            [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var scopes = await _scopeRepository.GetAllActiveScopesAsync();
        var result = scopes.Select(s => new ScopeDtoOut(s.Id, s.Name));
        return Ok(result);
    }
}
