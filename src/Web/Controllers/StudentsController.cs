using Domain.DomainExceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Web.Models;
using Web.Services.Api;
using Microsoft.Extensions.Localization;

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
        ILogger<StudentsController> logger,
        IStringLocalizer<BaseController> localizer) : base(logger, localizer)
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
                SchoolName = s.SchoolName ?? Localizer["Sense escola"].Value
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
            SetErrorMessage(Localizer["Error carregant els alumnes. Si us plau, intenta-ho de nou."].Value);
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
            return Unauthorized(new { error = Localizer["Accés no autoritzat."].Value });
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
                SetErrorMessage(Localizer["Alumne amb ID {0} no trobat.", id].Value);
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
                SchoolName = student.SchoolName ?? Localizer["Sense escola"].Value
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
            SetErrorMessage(Localizer["Alumne amb ID {0} no trobat.", id].Value);
            return Redirect("/Students");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error obtenint detalls de l'alumne {Id}", id);
            SetErrorMessage(Localizer["Error carregant els detalls de l'alumne."].Value);
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
                return BadRequest(new { error = Localizer["Si us plau, omple tots els camps obligatoris correctament."].Value });
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
                return Ok(new { message = Localizer["Alumne {0} {1} creat correctament.", model.FirstName, model.LastName].Value });
            }
            SetSuccessMessage(Localizer["Alumne {0} {1} creat correctament.", model.FirstName, model.LastName].Value);
            return Redirect("/Students");
        }
        catch (DuplicateEntityException ex)
        {
            Logger.LogWarning(ex, "Email duplicat al crear alumne");
            return BadRequest(new { error = Localizer["Ja existeix un usuari amb l'email {0}", model.Email].Value });
        }
        catch (NotFoundException ex)
        {
            Logger.LogWarning(ex, "Escola no trobada al crear alumne");
            return BadRequest(new { error = Localizer["L'escola seleccionada no existeix."].Value });
        }
        catch (ValidationException ex)
        {
            Logger.LogWarning(ex, "Error de validació al crear alumne");
            return BadRequest(new { error = Localizer["Error de validació: {0}", string.Join(", ", ex.Errors.SelectMany(e => e.Value))].Value });
        }
        catch (HttpRequestException ex) when (IsUnauthorized(ex))
        {
            Logger.LogWarning(ex, "Accés no autoritzat a l'API (crear alumne)");
            if (IsAjaxRequest())
            {
                return Unauthorized(new { error = Localizer["Accés no autoritzat. Torna a iniciar sessió."].Value });
            }
            SetErrorMessage(Localizer["Accés no autoritzat. Torna a iniciar sessió."].Value);
            return RedirectToAction("Login", "Auth");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creant alumne");
            return BadRequest(new { error = Localizer["Error creant l'alumne. Si us plau, intenta-ho de nou."].Value });
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var student = await _studentsApi.GetByIdAsync(id);
            if (student == null)
            {
                SetErrorMessage(Localizer["Alumne amb ID {0} no trobat.", id].Value);
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
            SetErrorMessage(Localizer["Alumne amb ID {0} no trobat.", id].Value);
            return Redirect("/Students");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error carregant alumne per editar {Id}", id);
            SetErrorMessage(Localizer["Error carregant l'alumne."].Value);
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
                SetErrorMessage(Localizer["Si us plau, omple tots els camps obligatoris correctament."].Value);
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

            SetSuccessMessage(Localizer["Alumne {0} {1} actualitzat correctament.", model.FirstName, model.LastName].Value);
            return Redirect("/Students");
        }
        catch (NotFoundException ex)
        {
            Logger.LogWarning(ex, "Alumne o escola no trobats al actualitzar");
            SetErrorMessage(Localizer["L'alumne o l'escola no existeixen."].Value);
            return Redirect("/Students");
        }
        catch (ValidationException ex)
        {
            Logger.LogWarning(ex, "Error de validació al actualitzar alumne");
            SetErrorMessage(Localizer["Error de validació: {0}", string.Join(", ", ex.Errors.SelectMany(e => e.Value))].Value);
            return Redirect("/Students");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error actualitzant alumne {Id}", model.Id);
            SetErrorMessage(Localizer["Error al actualitzar l'alumne. Si us plau, intenta-ho de nou."].Value);
            return Redirect("/Students");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _studentsApi.DeleteAsync(id);

            SetSuccessMessage(Localizer["Alumne esborrat correctament."].Value);
            return Redirect("/Students");
        }
        catch (NotFoundException ex)
        {
            Logger.LogWarning(ex, "Intent d'esborrar alumne inexistent: {Id}", id);
            SetErrorMessage(Localizer["L'alumne no existeix."].Value);
            return Redirect("/Students");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error esborrant alumne {Id}", id);
            SetErrorMessage(Localizer["Error al esborrar l'alumne. Pot tenir matrícules associades."].Value);
            return Redirect("/Students");
        }
    }
}
