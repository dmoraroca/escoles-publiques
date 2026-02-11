using Domain.DomainExceptions;
using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Web.Services.Api;
using Microsoft.Extensions.Localization;

using Microsoft.AspNetCore.Authorization;
namespace Web.Controllers;

/// <summary>
/// Controlador per gestionar les inscripcions dels alumnes.
/// </summary>
[Authorize]
public class EnrollmentsController : BaseController
{
    private readonly IEnrollmentsApiClient _enrollmentsApi;
    private readonly IStudentsApiClient _studentsApi;
    private readonly ISchoolsApiClient _schoolsApi;

    public EnrollmentsController(
        IEnrollmentsApiClient enrollmentsApi,
        IStudentsApiClient studentsApi,
        ISchoolsApiClient schoolsApi,
        ILogger<EnrollmentsController> logger,
        IStringLocalizer<BaseController> localizer) : base(logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null in EnrollmentsController"), localizer)
    {
        _enrollmentsApi = enrollmentsApi;
        _studentsApi = studentsApi;
        _schoolsApi = schoolsApi;
    }

    /// <summary>
    /// Mostra el llistat de totes les inscripcions.
    /// </summary>
    public async Task<IActionResult> Index()
    {
        try
        {
            var enrollments = await _enrollmentsApi.GetAllAsync();
            var viewModels = enrollments
                .Where(e => e != null)
                .Select(e => new EnrollmentViewModel
                {
                    Id = (int)e.Id,
                    StudentName = $"{e.StudentId}-{e.StudentName}",
                    AcademicYear = e.AcademicYear,
                    CourseName = e.CourseName,
                    Status = e.Status,
                    EnrolledAt = e.EnrolledAt,
                    SchoolId = (int)e.SchoolId,
                    SchoolName = e.SchoolName ?? string.Empty
                });

            var students = await _studentsApi.GetAllAsync();
            ViewBag.Students = students.Select(s => new StudentViewModel
            {
                Id = (int)s.Id,
                FirstName = s.FirstName,
                LastName = s.LastName
            }).ToList();

            var schools = await _schoolsApi.GetAllAsync();
            ViewBag.Schools = schools.Select(s => new SchoolViewModel
            {
                Id = (int)s.Id,
                Name = s.Name
            }).ToList();

            return View(viewModels);
        }
        catch (Npgsql.NpgsqlException)
        {
            // Error de connexió a la base de dades
            return View("~/Views/Shared/ErrorDb.cshtml");
        }
        catch (Exception ex)
        {
            var userName = User?.Identity?.IsAuthenticated == true ? User.Identity.Name : "Usuari no autenticat";
            var ip = HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "IP desconeguda";
            Logger.LogError(ex, "Error obtenint llista d'inscripcions. Usuari: {UserName}, IP: {IP}", userName, ip);
            SetErrorMessage("Error carregant les inscripcions. Si us plau, intenta-ho de nou.");
            return View("~/Views/Shared/ErrorDb.cshtml");
        }
    }

    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var enrollment = await _enrollmentsApi.GetByIdAsync(id);
            var viewModel = (enrollment == null)
                ? new EnrollmentViewModel()
                : new EnrollmentViewModel
                {
                    Id = (int)enrollment.Id,
                    StudentId = (int)enrollment.StudentId,
                    StudentName = enrollment.StudentName,
                    AcademicYear = enrollment.AcademicYear,
                    CourseName = enrollment.CourseName,
                    Status = enrollment.Status,
                    EnrolledAt = enrollment.EnrolledAt,
                    SchoolId = (int)enrollment.SchoolId,
                    SchoolName = enrollment.SchoolName
                };
            var students = await _studentsApi.GetAllAsync();
            ViewBag.Students = students.Select(s => new StudentViewModel
            {
                Id = (int)s.Id,
                FirstName = s.FirstName,
                LastName = s.LastName
            }).ToList();

            var schools = await _schoolsApi.GetAllAsync();
            var schoolList = schools.Select(s => new SchoolViewModel
            {
                Id = (int)s.Id,
                Name = s.Name
            }).ToList();
            ViewBag.Schools = schoolList;
            if (string.IsNullOrWhiteSpace(viewModel.SchoolName) && viewModel.SchoolId != 0)
            {
                viewModel.SchoolName = schoolList.FirstOrDefault(s => s.Id == viewModel.SchoolId)?.Name ?? string.Empty;
            }
            return View(viewModel);
        }
        catch (NotFoundException ex)
        {
            Logger.LogWarning(ex, "Inscripció amb Id {Id} no trobada", id);
            SetErrorMessage($"Inscripció amb ID {id} no trobada.");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error obtenint els detalls de la inscripció amb ID {Id}", id);
            SetErrorMessage("Error carregant els detalls de la inscripció.");
            return RedirectToAction(nameof(Index));
        }
    }

    public async Task<IActionResult> Create()
    {
        var students = await _studentsApi.GetAllAsync();
        ViewBag.Students = students.Select(s => new StudentViewModel
        {
            Id = (int)s.Id,
            FirstName = s.FirstName,
            LastName = s.LastName
        }).ToList();

        // Afegim escoles al ViewBag
        var schools = await _schoolsApi.GetAllAsync();
        ViewBag.Schools = schools.Select(s => new SchoolViewModel
        {
            Id = (int)s.Id,
            Name = s.Name
        }).ToList();

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EnrollmentViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Logger.LogWarning("ModelState error: {ErrorMessage}", error.ErrorMessage);
                }
                SetErrorMessage("Si us plau, omple tots els camps obligatoris correctament.");
                return RedirectToAction(nameof(Index));
            }
            if (model.SchoolId == 0)
            {
                Logger.LogWarning("SchoolId no pot ser zero al crear inscripció");
                SetErrorMessage("Has de seleccionar una escola per a la inscripció.");
                return RedirectToAction(nameof(Index));
            }

            var enrollment = new Domain.Entities.Enrollment
            {
                StudentId = model.StudentId,
                AcademicYear = model.AcademicYear,
                CourseName = model.CourseName,
                Status = model.Status,
                SchoolId = model.SchoolId
            };

            var dto = new ApiEnrollmentIn(
                model.StudentId,
                model.AcademicYear,
                model.CourseName,
                model.Status,
                null,
                model.SchoolId
            );
            await _enrollmentsApi.CreateAsync(dto);
            if (IsAjaxRequest())
            {
                return Ok(new { message = "Inscripció creada correctament." });
            }
            SetSuccessMessage("Inscripció creada correctament.");
            return RedirectToAction(nameof(Index));
        }
        catch (NotFoundException ex)
        {
            Logger.LogWarning(ex, "Alumne no trobat al crear inscripció");
            SetErrorMessage("L'alumne seleccionat no existeix.");
            return RedirectToAction(nameof(Index));
        }
        catch (ValidationException ex)
        {
            Logger.LogWarning(ex, "Error de validació al crear inscripció");
            SetErrorMessage($"Error de validació: {string.Join(", ", ex.Errors.SelectMany(e => e.Value))}");
            return RedirectToAction(nameof(Index));
        }
        catch (HttpRequestException ex) when (IsUnauthorized(ex))
        {
            Logger.LogWarning(ex, "Accés no autoritzat a l'API (crear inscripció)");
            if (IsAjaxRequest())
            {
                return Unauthorized(new { error = "Accés no autoritzat. Torna a iniciar sessió." });
            }
            SetErrorMessage("Accés no autoritzat. Torna a iniciar sessió.");
            return RedirectToAction("Login", "Auth");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creant inscripció");
            SetErrorMessage("Error creant la inscripció. Si us plau, intenta-ho de nou.");
            return RedirectToAction(nameof(Index));
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var enrollment = await _enrollmentsApi.GetByIdAsync(id);
            if (enrollment == null)
            {
                SetErrorMessage($"Inscripció amb ID {id} no trobada.");
                return RedirectToAction(nameof(Index));
            }

            var students = await _studentsApi.GetAllAsync();
            ViewBag.Students = students.Select(s => new StudentViewModel
            {
                Id = (int)s.Id,
                FirstName = s.FirstName,
                LastName = s.LastName
            }).ToList();

            // Afegim escoles al ViewBag
            var schools = await _schoolsApi.GetAllAsync();
            var schoolList = schools.Select(s => new SchoolViewModel
            {
                Id = (int)s.Id,
                Name = s.Name
            }).ToList();
            ViewBag.Schools = schoolList;

            var viewModel = new EnrollmentViewModel
            {
                Id = (int)enrollment.Id,
                StudentId = (int)enrollment.StudentId,
                StudentName = enrollment.StudentName,
                AcademicYear = enrollment.AcademicYear,
                CourseName = enrollment.CourseName,
                Status = enrollment.Status,
                SchoolId = (int)enrollment.SchoolId,
                SchoolName = enrollment.SchoolName ?? ""
            };
            if (string.IsNullOrWhiteSpace(viewModel.SchoolName) && viewModel.SchoolId != 0)
            {
                viewModel.SchoolName = schoolList.FirstOrDefault(s => s.Id == viewModel.SchoolId)?.Name ?? string.Empty;
            }

            return View(viewModel);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error carregant inscripció per editar {Id}", id);
            SetErrorMessage("Error carregant la inscripció.");
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EnrollmentViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                SetErrorMessage("Si us plau, omple tots els camps obligatoris correctament.");
                var students = await _studentsApi.GetAllAsync();
                ViewBag.Students = students.Select(s => new StudentViewModel
                {
                    Id = (int)s.Id,
                    FirstName = s.FirstName,
                    LastName = s.LastName
                }).ToList();
                var schools = await _schoolsApi.GetAllAsync();
                var schoolList = schools.Select(s => new SchoolViewModel
                {
                    Id = (int)s.Id,
                    Name = s.Name
                }).ToList();
                ViewBag.Schools = schoolList;
                if (string.IsNullOrWhiteSpace(model.SchoolName) && model.SchoolId != 0)
                {
                    model.SchoolName = schoolList.FirstOrDefault(s => s.Id == model.SchoolId)?.Name ?? string.Empty;
                }
                return View(model);
            }

            var dto = new ApiEnrollmentIn(
                model.StudentId,
                model.AcademicYear,
                model.CourseName,
                model.Status,
                model.EnrolledAt,
                model.SchoolId
            );
            await _enrollmentsApi.UpdateAsync(model.Id, dto);
            SetSuccessMessage("Inscripció actualitzada correctament.");
            return RedirectToAction(nameof(Details), new { id = model.Id });
        }
        catch (NotFoundException ex)
        {
            Logger.LogWarning(ex, "Inscripció o alumne no trobats al actualitzar");
            SetErrorMessage("La inscripció o l'alumne no existeixen.");
            return RedirectToAction(nameof(Index));
        }
        catch (ValidationException ex)
        {
            Logger.LogWarning(ex, "Error de validació al actualitzar inscripció");
            SetErrorMessage($"Error de validació: {string.Join(", ", ex.Errors.SelectMany(e => e.Value))}");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error actualitzant inscripció {Id}", model.Id);
            SetErrorMessage("Error al actualitzar la inscripció. Si us plau, intenta-ho de nou.");
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _enrollmentsApi.DeleteAsync(id);

            SetSuccessMessage("Inscripció esborrada correctament.");
            return RedirectToAction(nameof(Index));
        }
        catch (NotFoundException ex)
        {
            Logger.LogWarning(ex, "Intent d'esborrar inscripció inexistent: {Id}", id);
            SetErrorMessage("La inscripció no existeix.");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error esborrant inscripció {Id}", id);
            SetErrorMessage("Error al esborrar la inscripció. Pot tenir quotes associades.");
            return RedirectToAction(nameof(Index));
        }
    }
}
