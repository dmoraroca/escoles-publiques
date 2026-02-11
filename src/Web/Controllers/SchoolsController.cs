using Application.Interfaces;
using Web.Services.Api;
using Domain.DomainExceptions;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using Web.Hubs;
using Web.Models;
using Web.Services.Api;
using Microsoft.Extensions.Localization;

using Microsoft.AspNetCore.Authorization;
namespace Web.Controllers;

/// <summary>
/// Controlador per gestionar les escoles: llistat, detalls, creació, edició i eliminació.
/// </summary>
/// <summary>
/// Controlador per gestionar les escoles: llistat, detalls, creació, edició i eliminació.
/// </summary>
[Authorize]
public class SchoolsController : BaseController
{
    private readonly ISchoolsApiClient _schoolApi;
    private readonly IHubContext<SchoolHub> _hubContext;
    private readonly IScopesApiClient _scopesApi;

    /// <summary>
    /// Constructor del controlador d'escoles.
    /// </summary>
    /// <summary>
    /// Constructor del controlador d'escoles.
    /// </summary>
    public SchoolsController(
        ISchoolsApiClient schoolApi,
        IHubContext<SchoolHub> hubContext,
        IScopesApiClient scopesApi,
        ILogger<SchoolsController> logger,
        IStringLocalizer<BaseController> localizer) : base(logger, localizer)
    {
        _schoolApi = schoolApi;
        _hubContext = hubContext;
        _scopesApi = scopesApi;
    }

    /// <summary>
    /// Mostra el llistat de totes les escoles.
    /// </summary>
    /// <summary>
    /// Mostra el llistat de totes les escoles.
    /// </summary>
    public async Task<IActionResult> Index()
    {
        try
        {
            var schools = await _schoolApi.GetAllAsync();
            var scopes = (await _scopesApi.GetAllAsync()).ToList();
            var viewModels = schools.Select(s => new SchoolViewModel
            {
                Id = (int)s.Id,
                Code = s.Code,
                Name = s.Name,
                City = s.City ?? "",
                IsFavorite = s.IsFavorite,
                ScopeId = s.ScopeId,
                ScopeName = s.ScopeId.HasValue ? scopes.FirstOrDefault(sc => sc.Id == s.ScopeId)?.Name : null,
                CreatedAt = s.CreatedAt
            });
            ViewBag.Scopes = scopes.Select(s => new SelectOption
            {
                Value = s.Id.ToString(),
                Text = s.Name
            }).ToList();
            var role = User.FindFirstValue("Role");
            ViewBag.UserRole = role;
            return View(viewModels);
        }
        catch (HttpRequestException ex) when (IsUnauthorized(ex))
        {
            Logger.LogWarning(ex, "Accés no autoritzat a l'API (llistat escoles)");
            SetErrorMessage(Localizer["Accés no autoritzat. Torna a iniciar sessió."].Value);
            return View(new List<SchoolViewModel>());
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error obtenint llista d'escoles");
            SetErrorMessage(Localizer["Error carregant les escoles. Si us plau, intenta-ho de nou."].Value);
            return View(new List<SchoolViewModel>());
        }
    }

    /// <summary>
    /// Mostra els detalls d'una escola concreta.
    /// </summary>
    /// <summary>
    /// Mostra els detalls d'una escola concreta.
    /// </summary>
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var school = await _schoolApi.GetByIdAsync(id);

            var scopes = await _scopesApi.GetAllAsync();
            var scopeName = school.ScopeId.HasValue ? scopes.FirstOrDefault(sc => sc.Id == school.ScopeId)?.Name : null;
            var viewModel = new SchoolViewModel
            {
                Id = (int)school.Id,
                Code = school.Code,
                Name = school.Name,
                City = school.City ?? "",
                IsFavorite = school.IsFavorite,
                ScopeId = school.ScopeId,
                ScopeName = scopeName,
                CreatedAt = school.CreatedAt
            };
            ViewBag.Scopes = scopes.Select(s => new SelectOption { Value = s.Id.ToString(), Text = s.Name }).ToList();
            return View(viewModel);
        }
        catch (HttpRequestException ex) when (IsUnauthorized(ex))
        {
            Logger.LogWarning(ex, "Accés no autoritzat a l'API (detalls escola {Id})", id);
            SetErrorMessage(Localizer["Accés no autoritzat. Torna a iniciar sessió."].Value);
            return RedirectToAction(nameof(Index));
        }
        catch (NotFoundException ex)
        {
            Logger.LogWarning(ex, "Escola amb Id {Id} no trobada", id);
            SetErrorMessage(Localizer["Escola amb ID {0} no trobada.", id].Value);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error obtenint detalls de l'escola {Id}", id);
            SetErrorMessage(Localizer["Error carregant els detalls de l'escola."].Value);
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Mostra el formulari per crear una nova escola.
    /// </summary>
    /// <summary>
    /// Mostra el formulari per crear una nova escola.
    /// </summary>
    public async Task<IActionResult> Create()
    {
        var scopes = await _scopesApi.GetAllAsync();
        ViewBag.Scopes = scopes.Select(s => new SelectOption { Value = s.Id.ToString(), Text = s.Name }).ToList();
        return View(new SchoolViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /// <summary>
    /// Processa la creació d'una nova escola.
    /// </summary>
    /// <summary>
    /// Processa la creació d'una nova escola.
    /// </summary>
    public async Task<IActionResult> Create(SchoolViewModel model)
    {
        try
        {
            // Normalitza l'àmbit abans de validar (p.ex. si arriba com a nom en lloc d'Id)
            if (!model.ScopeId.HasValue || ModelState.ContainsKey(nameof(SchoolViewModel.ScopeId)))
            {
                if (Request.Form.TryGetValue("ScopeId", out var scopeIdRaw)
                    && long.TryParse(scopeIdRaw, out var parsedScopeId))
                {
                    model.ScopeId = parsedScopeId;
                    ModelState.Remove(nameof(SchoolViewModel.ScopeId));
                }
                else if (Request.Form.TryGetValue("Scope", out var scopeRaw)
                    && long.TryParse(scopeRaw, out var parsedLegacyScopeId))
                {
                    model.ScopeId = parsedLegacyScopeId;
                    ModelState.Remove(nameof(SchoolViewModel.ScopeId));
                }
                else if (!string.IsNullOrWhiteSpace(scopeIdRaw))
                {
                    var scopes = await _scopesApi.GetAllAsync();
                    var matched = scopes.FirstOrDefault(s =>
                        string.Equals(s.Name, scopeIdRaw.ToString(), StringComparison.OrdinalIgnoreCase));
                    if (matched != null)
                    {
                        model.ScopeId = matched.Id;
                        ModelState.Remove(nameof(SchoolViewModel.ScopeId));
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                foreach (var entry in ModelState)
                {
                    var errors = entry.Value?.Errors;
                    if (errors == null || errors.Count == 0) continue;
                    foreach (var error in errors)
                    {
                        Logger.LogWarning("ModelState error on {Field}: {Message}", entry.Key, error.ErrorMessage);
                    }
                }
                SetErrorMessage(Localizer["Si us plau, omple tots els camps obligatoris."].Value);
                var scopes = await _scopesApi.GetAllAsync();
                ViewBag.Scopes = scopes.Select(s => new SelectOption { Value = s.Id.ToString(), Text = s.Name }).ToList();
                return View(model);
            }

            Logger.LogInformation("Create school form ScopeId raw={ScopeIdRaw} parsed={ScopeId} name={ScopeName}",
                Request.Form.TryGetValue("ScopeId", out var scopeIdRawLog) ? scopeIdRawLog.ToString() : "",
                model.ScopeId,
                Request.Form.TryGetValue("ScopeId", out var scopeNameRawLog) ? scopeNameRawLog.ToString() : "");

            var school = new Domain.Entities.School
            {
                Code = model.Code,
                Name = model.Name,
                City = model.City,
                IsFavorite = model.IsFavorite,
                ScopeId = model.ScopeId
            };
            var created = await _schoolApi.CreateAsync(school);
            var scopeName = string.Empty;
            if (model.ScopeId.HasValue)
            {
                var scope = (await _scopesApi.GetAllAsync()).FirstOrDefault(s => s.Id == model.ScopeId);
                scopeName = scope?.Name ?? string.Empty;
            }
            await _hubContext.Clients.All.SendAsync("SchoolCreated", new
            {
                id = school.Id,
                code = school.Code,
                name = school.Name,
                city = school.City,
                scopeId = school.ScopeId,
                scopeName = scopeName,
                isFavorite = school.IsFavorite,
                createdAt = school.CreatedAt.ToString("dd/MM/yyyy HH:mm")
            });
            if (IsAjaxRequest())
            {
                return Ok(new { message = Localizer["Escola '{0}' creada correctament.", school.Name].Value });
            }
            SetSuccessMessage(Localizer["Escola '{0}' creada correctament.", school.Name].Value);
            return RedirectToAction(nameof(Index));
        }
        catch (HttpRequestException ex) when (IsUnauthorized(ex))
        {
            Logger.LogWarning(ex, "Accés no autoritzat a l'API (crear escola)");
            if (IsAjaxRequest())
            {
                return Unauthorized(new { error = Localizer["Accés no autoritzat. Torna a iniciar sessió."].Value });
            }
            SetErrorMessage(Localizer["Accés no autoritzat. Torna a iniciar sessió."].Value);
            return RedirectToAction("Login", "Auth");
        }
        catch (DuplicateEntityException ex)
        {
            Logger.LogWarning(ex, "Intent de crear escola amb codi duplicat: {Code}", model.Code);
            SetErrorMessage(Localizer["Ja existeix una escola amb el codi '{0}'.", model.Code].Value);
            return RedirectToAction(nameof(Index));
        }
        catch (ValidationException ex)
        {
            Logger.LogWarning(ex, "Validació fallida al crear escola");
            SetErrorMessage(Localizer["Error de validació: {0}", ex.Message].Value);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creant escola");
            SetErrorMessage(Localizer["Error al crear l'escola. Si us plau, intenta-ho de nou."].Value);
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Mostra el formulari per editar una escola existent.
    /// </summary>
    /// <summary>
    /// Mostra el formulari per editar una escola existent.
    /// </summary>
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var school = await _schoolApi.GetByIdAsync(id);

            var scopes = await _scopesApi.GetAllAsync();
            var scopeName = school.ScopeId.HasValue ? scopes.FirstOrDefault(sc => sc.Id == school.ScopeId)?.Name : null;
            var viewModel = new SchoolViewModel
            {
                Id = (int)school.Id,
                Code = school.Code,
                Name = school.Name,
                City = school.City ?? "",
                IsFavorite = school.IsFavorite,
                ScopeId = school.ScopeId,
                ScopeName = scopeName,
                CreatedAt = school.CreatedAt
            };
            ViewBag.Scopes = scopes.Select(s => new SelectOption { Value = s.Id.ToString(), Text = s.Name }).ToList();
            return View(viewModel);
        }
        catch (HttpRequestException ex) when (IsUnauthorized(ex))
        {
            Logger.LogWarning(ex, "Accés no autoritzat a l'API (editar escola {Id})", id);
            SetErrorMessage(Localizer["Accés no autoritzat. Torna a iniciar sessió."].Value);
            return RedirectToAction(nameof(Index));
        }
        catch (NotFoundException ex)
        {
            Logger.LogWarning(ex, "Escola amb Id {Id} no trobada per editar", id);
            SetErrorMessage(Localizer["Escola amb ID {0} no trobada.", id].Value);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error carregant escola per editar {Id}", id);
            SetErrorMessage(Localizer["Error carregant l'escola."].Value);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /// <summary>
    /// Processa l'edició d'una escola existent.
    /// </summary>
    /// <summary>
    /// Processa l'edició d'una escola existent.
    /// </summary>
    public async Task<IActionResult> Edit(SchoolViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                SetErrorMessage(Localizer["Si us plau, omple tots els camps obligatoris."].Value);
                var scopes = await _scopesApi.GetAllAsync();
                ViewBag.Scopes = scopes.Select(s => new SelectOption { Value = s.Id.ToString(), Text = s.Name }).ToList();
                return View(model);
            }

            var school = await _schoolApi.GetByIdAsync(model.Id);

            school.Code = model.Code;
            school.Name = model.Name;
            school.City = model.City;
            school.IsFavorite = model.IsFavorite;
            school.ScopeId = model.ScopeId;

            await _schoolApi.UpdateAsync(school.Id, school);

            var scopeName = string.Empty;
            if (school.ScopeId.HasValue)
            {
                var scope = (await _scopesApi.GetAllAsync()).FirstOrDefault(s => s.Id == school.ScopeId);
                scopeName = scope?.Name ?? string.Empty;
            }
            await _hubContext.Clients.All.SendAsync("SchoolUpdated", new
            {
                id = school.Id,
                code = school.Code,
                name = school.Name,
                city = school.City,
                scopeId = school.ScopeId,
                scopeName = scopeName,
                isFavorite = school.IsFavorite,
                createdAt = school.CreatedAt.ToString("dd/MM/yyyy HH:mm")
            });

            SetSuccessMessage(Localizer["Escola '{0}' actualitzada correctament.", school.Name].Value);
            return RedirectToAction(nameof(Details), new { id = model.Id });
        }
        catch (HttpRequestException ex) when (IsUnauthorized(ex))
        {
            Logger.LogWarning(ex, "Accés no autoritzat a l'API (actualitzar escola {Id})", model.Id);
            SetErrorMessage(Localizer["Accés no autoritzat. Torna a iniciar sessió."].Value);
            return RedirectToAction("Login", "Auth");
        }
        catch (NotFoundException ex)
        {
            Logger.LogWarning(ex, "Escola no trobada al actualitzar: {Id}", model.Id);
            SetErrorMessage(Localizer["L'escola no existeix."].Value);
            return RedirectToAction(nameof(Index));
        }
        catch (DuplicateEntityException ex)
        {
            Logger.LogWarning(ex, "Intent d'actualitzar amb codi duplicat: {Code}", model.Code);
            SetErrorMessage(Localizer["Ja existeix una altra escola amb el codi '{0}'.", model.Code].Value);
            var scopes = await _scopesApi.GetAllAsync();
            ViewBag.Scopes = scopes.Select(s => new SelectOption { Value = s.Id.ToString(), Text = s.Name }).ToList();
            return View(model);
        }
        catch (ValidationException ex)
        {
            Logger.LogWarning(ex, "Validació fallida al actualitzar escola");
            SetErrorMessage(Localizer["Error de validació: {0}", ex.Message].Value);
            var scopes = await _scopesApi.GetAllAsync();
            ViewBag.Scopes = scopes.Select(s => new SelectOption { Value = s.Id.ToString(), Text = s.Name }).ToList();
            return View(model);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error actualitzant escola {Id}", model.Id);
            SetErrorMessage(Localizer["Error al actualitzar l'escola. Si us plau, intenta-ho de nou."].Value);
            return View(model);
        }
    }

    [HttpPost]
    /// <summary>
    /// Elimina una escola.
    /// </summary>
    /// <summary>
    /// Elimina una escola.
    /// </summary>
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var school = await _schoolApi.GetByIdAsync(id);
            var code = school?.Code;

            await _schoolApi.DeleteAsync(id);

            await _hubContext.Clients.All.SendAsync("SchoolDeleted", new { id, code });

            SetSuccessMessage(Localizer["Escola esborrada correctament."].Value);
            return RedirectToAction(nameof(Index));
        }
        catch (HttpRequestException ex) when (IsUnauthorized(ex))
        {
            Logger.LogWarning(ex, "Accés no autoritzat a l'API (esborrar escola {Id})", id);
            SetErrorMessage(Localizer["Accés no autoritzat. Torna a iniciar sessió."].Value);
            return RedirectToAction("Login", "Auth");
        }
        catch (NotFoundException ex)
        {
            Logger.LogWarning(ex, "Intent d'esborrar escola inexistent: {Id}", id);
            SetErrorMessage(Localizer["L'escola no existeix."].Value);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error esborrant escola {Id}", id);
            SetErrorMessage(Localizer["Error al esborrar l'escola. Pot tenir dades relacionades."].Value);
            return RedirectToAction(nameof(Index));
        }
    }
}
