using Domain.DomainExceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Web.Models;
using Web.Services.Api;

namespace Web.Controllers;

/// <summary>
/// Controlador per gestionar els alumnes del sistema.
/// </summary>
[Authorize]
public class StudentsController : BaseController
{
    private readonly IStudentsApiClient _studentsApi;
    private readonly ISchoolsApiClient _schoolsApi;

    public StudentsController(
        IStudentsApiClient studentsApi,
        ISchoolsApiClient schoolsApi,
        ILogger<StudentsController> logger) : base(logger)
    {
        _studentsApi = studentsApi;
        _schoolsApi = schoolsApi;
    }
    
    /// <summary>
    /// Mostra el llistat de tots els alumnes.
    /// </summary>
    public async Task<IActionResult> Index()
    {
        try
        {
            var students = await _studentsApi.GetAllAsync();
            var viewModels = students.Select(s => new StudentViewModel
            {
                Id = (int)s.Id,
                UserId = s.UserId,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Email = s.Email,
                BirthDate = s.BirthDate.HasValue ? s.BirthDate.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
                SchoolId = (int)s.SchoolId,
                SchoolName = s.SchoolName ?? "Sense escola"
            });

            var schools = await _schoolsApi.GetAllAsync();
            ViewBag.Schools = schools.Select(s => new SchoolViewModel
            {
                Id = (int)s.Id,
                Name = s.Name
            }).ToList();
            
            return View(viewModels.ToList());
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error obtenint llista d'alumnes");
            SetErrorMessage("Error carregant els alumnes. Si us plau, intenta-ho de nou.");
            return View(new List<StudentViewModel>());
        }
    }

    [HttpGet]
    public async Task<IActionResult> CheckEmail(string email)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return Ok(new { exists = false });
            }

            var students = await _studentsApi.GetAllAsync();
            var exists = students.Any(s =>
                !string.IsNullOrWhiteSpace(s.Email) &&
                string.Equals(s.Email.Trim(), email.Trim(), StringComparison.OrdinalIgnoreCase));

            return Ok(new { exists });
        }
        catch (HttpRequestException ex) when (IsUnauthorized(ex))
        {
            Logger.LogWarning(ex, "Accés no autoritzat a l'API (check email alumne)");
            return Unauthorized(new { error = "Accés no autoritzat." });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error comprovant email d'alumne");
            return Ok(new { exists = false });
        }
    }
    
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var student = await _studentsApi.GetByIdAsync(id);
            if (student == null)
            {
                SetErrorMessage($"Alumne amb ID {id} no trobat.");
                return Redirect("/Students");
            }
            
            var viewModel = new StudentViewModel
            {
                Id = (int)student.Id,
                UserId = student.UserId,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Email = student.Email,
                BirthDate = student.BirthDate.HasValue ? student.BirthDate.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
                SchoolId = (int)student.SchoolId,
                SchoolName = student.SchoolName ?? "Sense escola"
            };
            
            var schools = await _schoolsApi.GetAllAsync();
            ViewBag.Schools = schools.Select(s => new SchoolViewModel
            {
                Id = (int)s.Id,
                Name = s.Name
            }).ToList();
            
            return View(viewModel);
        }
        catch (NotFoundException ex)
        {
            Logger.LogWarning(ex, "Alumne amb Id {Id} no trobat", id);
            SetErrorMessage($"Alumne amb ID {id} no trobat.");
            return Redirect("/Students");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error obtenint detalls de l'alumne {Id}", id);
            SetErrorMessage("Error carregant els detalls de l'alumne.");
            return Redirect("/Students");
        }
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(StudentViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { error = "Si us plau, omple tots els camps obligatoris correctament." });
            }

            var dto = new ApiStudentIn(
                model.FirstName,
                model.LastName,
                model.Email,
                model.BirthDate.HasValue ? DateOnly.FromDateTime(model.BirthDate.Value) : null,
                model.SchoolId
            );
            await _studentsApi.CreateAsync(dto);

            if (IsAjaxRequest())
            {
                return Ok(new { message = $"Alumne {model.FirstName} {model.LastName} creat correctament." });
            }
            SetSuccessMessage($"Alumne {model.FirstName} {model.LastName} creat correctament.");
            return Redirect("/Students");
        }
        catch (DuplicateEntityException ex)
        {
            Logger.LogWarning(ex, "Email duplicat al crear alumne");
            return BadRequest(new { error = $"Ja existeix un usuari amb l'email {model.Email}" });
        }
        catch (NotFoundException ex)
        {
            Logger.LogWarning(ex, "Escola no trobada al crear alumne");
            return BadRequest(new { error = "L'escola seleccionada no existeix." });
        }
        catch (ValidationException ex)
        {
            Logger.LogWarning(ex, "Error de validació al crear alumne");
            return BadRequest(new { error = $"Error de validació: {string.Join(", ", ex.Errors.SelectMany(e => e.Value))}" });
        }
        catch (HttpRequestException ex) when (IsUnauthorized(ex))
        {
            Logger.LogWarning(ex, "Accés no autoritzat a l'API (crear alumne)");
            if (IsAjaxRequest())
            {
                return Unauthorized(new { error = "Accés no autoritzat. Torna a iniciar sessió." });
            }
            SetErrorMessage("Accés no autoritzat. Torna a iniciar sessió.");
            return RedirectToAction("Login", "Auth");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creant alumne");
            return BadRequest(new { error = "Error creant l'alumne. Si us plau, intenta-ho de nou." });
        }
    }
    
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var student = await _studentsApi.GetByIdAsync(id);
            if (student == null)
            {
                SetErrorMessage($"Alumne amb ID {id} no trobat.");
                return Redirect("/Students");
            }
            
            var viewModel = new StudentViewModel
            {
                Id = (int)student.Id,
                UserId = student.UserId,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Email = student.Email,
                BirthDate = student.BirthDate.HasValue ? student.BirthDate.Value.ToDateTime(TimeOnly.MinValue) : null,
                SchoolId = (int)student.SchoolId,
                SchoolName = student.SchoolName ?? ""
            };
            
            var schools = await _schoolsApi.GetAllAsync();
            ViewBag.Schools = schools.Select(s => new SchoolViewModel
            {
                Id = (int)s.Id,
                Name = s.Name
            }).ToList();
            
            return View(viewModel);
        }
        catch (NotFoundException ex)
        {
            Logger.LogWarning(ex, "Alumne amb Id {Id} no trobat per editar", id);
            SetErrorMessage($"Alumne amb ID {id} no trobat.");
            return Redirect("/Students");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error carregant alumne per editar {Id}", id);
            SetErrorMessage("Error carregant l'alumne.");
            return Redirect("/Students");
        }
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(StudentViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                SetErrorMessage("Si us plau, omple tots els camps obligatoris correctament.");
                var schools = await _schoolsApi.GetAllAsync();
                ViewBag.Schools = schools.Select(s => new SchoolViewModel
                {
                    Id = (int)s.Id,
                    Name = s.Name
                }).ToList();
                return View(model);
            }

            var dto = new ApiStudentIn(
                model.FirstName,
                model.LastName,
                model.Email,
                model.BirthDate.HasValue ? DateOnly.FromDateTime(model.BirthDate.Value) : null,
                model.SchoolId
            );
            await _studentsApi.UpdateAsync(model.Id, dto);
            
            SetSuccessMessage($"Alumne {model.FirstName} {model.LastName} actualitzat correctament.");
            return Redirect("/Students");
        }
        catch (NotFoundException ex)
        {
            Logger.LogWarning(ex, "Alumne o escola no trobats al actualitzar");
            SetErrorMessage("L'alumne o l'escola no existeixen.");
            return Redirect("/Students");
        }
        catch (ValidationException ex)
        {
            Logger.LogWarning(ex, "Error de validació al actualitzar alumne");
            SetErrorMessage($"Error de validació: {string.Join(", ", ex.Errors.SelectMany(e => e.Value))}");
            return Redirect("/Students");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error actualitzant alumne {Id}", model.Id);
            SetErrorMessage("Error al actualitzar l'alumne. Si us plau, intenta-ho de nou.");
            return Redirect("/Students");
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _studentsApi.DeleteAsync(id);
            
            SetSuccessMessage("Alumne esborrat correctament.");
            return Redirect("/Students");
        }
        catch (NotFoundException ex)
        {
            Logger.LogWarning(ex, "Intent d'esborrar alumne inexistent: {Id}", id);
            SetErrorMessage("L'alumne no existeix.");
            return Redirect("/Students");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error esborrant alumne {Id}", id);
            SetErrorMessage("Error al esborrar l'alumne. Pot tenir matrícules associades.");
            return Redirect("/Students");
        }
    }
}
