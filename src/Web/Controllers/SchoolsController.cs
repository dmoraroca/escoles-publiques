using Application.Interfaces;
using Domain.DomainExceptions;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using Web.Hubs;
using Web.Models;

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
    private readonly ISchoolService _schoolService;
    private readonly IHubContext<SchoolHub> _hubContext;
    private readonly IScopeRepository _scopeRepository;

    /// <summary>
    /// Constructor del controlador d'escoles.
    /// </summary>
    /// <summary>
    /// Constructor del controlador d'escoles.
    /// </summary>
    public SchoolsController(ISchoolService schoolService, IHubContext<SchoolHub> hubContext, IScopeRepository scopeRepository, ILogger<SchoolsController> logger) : base(logger)
    {
        _schoolService = schoolService;
        _hubContext = hubContext;
        _scopeRepository = scopeRepository;
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
            var schools = await _schoolService.GetAllSchoolsAsync();
            var viewModels = schools.Select(s => new SchoolViewModel
            {
                Id = (int)s.Id,
                Code = s.Code,
                Name = s.Name,
                City = s.City ?? "",
                IsFavorite = s.IsFavorite,
                Scope = s.Scope,
                CreatedAt = s.CreatedAt
            });
            
            var scopes = await _scopeRepository.GetAllActiveScopesAsync();
            ViewBag.Scopes = scopes.Select(s => new SelectOption
            {
                Value = s.Name,
                Text = s.Name
            }).ToList();
            
            var role = User.FindFirstValue("Role");
            ViewBag.UserRole = role;
            return View(viewModels);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error obtenint llista d'escoles");
            SetErrorMessage("Error carregant les escoles. Si us plau, intenta-ho de nou.");
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
            var school = await _schoolService.GetSchoolByIdAsync(id);
            
            var viewModel = new SchoolViewModel
            {
                Id = (int)school.Id,
                Code = school.Code,
                Name = school.Name,
                City = school.City ?? "",
                IsFavorite = school.IsFavorite,
                Scope = school.Scope,
                CreatedAt = school.CreatedAt
            };
            
            var scopes = await _scopeRepository.GetAllActiveScopesAsync();
            ViewBag.Scopes = scopes.Select(s => s.Name).ToList();
            
            return View(viewModel);
        }
        catch (NotFoundException ex)
        {
            Logger.LogWarning(ex, "Escola amb Id {Id} no trobada", id);
            SetErrorMessage($"Escola amb ID {id} no trobada.");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error obtenint detalls de l'escola {Id}", id);
            SetErrorMessage("Error carregant els detalls de l'escola.");
            return RedirectToAction(nameof(Index));
        }
    }
    
    /// <summary>
    /// Mostra el formulari per crear una nova escola.
    /// </summary>
    /// <summary>
    /// Mostra el formulari per crear una nova escola.
    /// </summary>
    public IActionResult Create()
    {
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
            if (!ModelState.IsValid)
            {
                SetErrorMessage("Si us plau, omple tots els camps obligatoris.");
                return RedirectToAction(nameof(Index));
            }

            var school = new Domain.Entities.School
            {
                Code = model.Code,
                Name = model.Name,
                City = model.City,
                IsFavorite = model.IsFavorite,
                Scope = model.Scope
            };

            await _schoolService.CreateSchoolAsync(school);
            
            await _hubContext.Clients.All.SendAsync("SchoolCreated", new 
            { 
                id = school.Id,
                code = school.Code, 
                name = school.Name, 
                city = school.City,
                scope = school.Scope,
                isFavorite = school.IsFavorite,
                createdAt = school.CreatedAt.ToString("dd/MM/yyyy HH:mm")
            });
            
            SetSuccessMessage($"Escola '{school.Name}' creada correctament.");
            return RedirectToAction(nameof(Index));
        }
        catch (DuplicateEntityException ex)
        {
            Logger.LogWarning(ex, "Intent de crear escola amb codi duplicat: {Code}", model.Code);
            SetErrorMessage($"Ja existeix una escola amb el codi '{model.Code}'.");
            return RedirectToAction(nameof(Index));
        }
        catch (ValidationException ex)
        {
            Logger.LogWarning(ex, "Validació fallida al crear escola");
            SetErrorMessage($"Error de validació: {ex.Message}");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creant escola");
            SetErrorMessage("Error al crear l'escola. Si us plau, intenta-ho de nou.");
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
            var school = await _schoolService.GetSchoolByIdAsync(id);
            
            var viewModel = new SchoolViewModel
            {
                Id = (int)school.Id,
                Code = school.Code,
                Name = school.Name,
                City = school.City ?? "",
                IsFavorite = school.IsFavorite,
                Scope = school.Scope,
                CreatedAt = school.CreatedAt
            };
            
            return View(viewModel);
        }
        catch (NotFoundException ex)
        {
            Logger.LogWarning(ex, "Escola amb Id {Id} no trobada per editar", id);
            SetErrorMessage($"Escola amb ID {id} no trobada.");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error carregant escola per editar {Id}", id);
            SetErrorMessage("Error carregant l'escola.");
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
                SetErrorMessage("Si us plau, omple tots els camps obligatoris.");
                return View(model);
            }

            var school = await _schoolService.GetSchoolByIdAsync(model.Id);
            
            school.Code = model.Code;
            school.Name = model.Name;
            school.City = model.City;
            school.IsFavorite = model.IsFavorite;
            school.Scope = model.Scope;

            await _schoolService.UpdateSchoolAsync(school);
            
            await _hubContext.Clients.All.SendAsync("SchoolUpdated", new 
            { 
                id = school.Id,
                code = school.Code, 
                name = school.Name, 
                city = school.City,
                scope = school.Scope,
                isFavorite = school.IsFavorite,
                createdAt = school.CreatedAt.ToString("dd/MM/yyyy HH:mm")
            });
            
            SetSuccessMessage($"Escola '{school.Name}' actualitzada correctament.");
            return RedirectToAction(nameof(Details), new { id = model.Id });
        }
        catch (NotFoundException ex)
        {
            Logger.LogWarning(ex, "Escola no trobada al actualitzar: {Id}", model.Id);
            SetErrorMessage("L'escola no existeix.");
            return RedirectToAction(nameof(Index));
        }
        catch (DuplicateEntityException ex)
        {
            Logger.LogWarning(ex, "Intent d'actualitzar amb codi duplicat: {Code}", model.Code);
            SetErrorMessage($"Ja existeix una altra escola amb el codi '{model.Code}'.");
            return View(model);
        }
        catch (ValidationException ex)
        {
            Logger.LogWarning(ex, "Validació fallida al actualitzar escola");
            SetErrorMessage($"Error de validació: {ex.Message}");
            return View(model);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error actualitzant escola {Id}", model.Id);
            SetErrorMessage("Error al actualitzar l'escola. Si us plau, intenta-ho de nou.");
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
            var school = await _schoolService.GetSchoolByIdAsync(id);
            var code = school.Code;
            
            await _schoolService.DeleteSchoolAsync(id);
            
            await _hubContext.Clients.All.SendAsync("SchoolDeleted", new { id, code });
            
            SetSuccessMessage("Escola esborrada correctament.");
            return RedirectToAction(nameof(Index));
        }
        catch (NotFoundException ex)
        {
            Logger.LogWarning(ex, "Intent d'esborrar escola inexistent: {Id}", id);
            SetErrorMessage("L'escola no existeix.");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error esborrant escola {Id}", id);
            SetErrorMessage("Error al esborrar l'escola. Pot tenir dades relacionades.");
            return RedirectToAction(nameof(Index));
        }
    }
}
