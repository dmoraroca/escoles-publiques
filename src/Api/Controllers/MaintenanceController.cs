using Api.Services;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADM")]
public class MaintenanceController : ControllerBase
{
    private readonly SchoolDbContext _db;
    private readonly IConfiguration _config;
    private readonly ILogger<MaintenanceController> _logger;

    public MaintenanceController(SchoolDbContext db, IConfiguration config, ILogger<MaintenanceController> logger)
    {
        _db = db;
        _config = config;
        _logger = logger;
    }

    // POST /api/maintenance/seed
    // Extra safety: requires X-Seed-Key header matching Seed:Key (Seed__Key env var).
    [HttpPost("seed")]
    public IActionResult Seed([FromHeader(Name = "X-Seed-Key")] string? seedKey)
    {
        var expectedKey = _config.GetValue<string>("Seed:Key");
        if (string.IsNullOrWhiteSpace(expectedKey))
        {
            return StatusCode(StatusCodes.Status409Conflict, "Seed key not configured. Set Seed__Key to enable seeding.");
        }

        if (!string.Equals(seedKey, expectedKey, StringComparison.Ordinal))
        {
            return Unauthorized("Invalid seed key.");
        }

        var seeded = DbSeeder.SeedIfEmpty(_db, _config, _logger);
        return Ok(new { seeded });
    }
}
