using Application.Interfaces;
using Domain.DomainExceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Web.Models;

namespace Web.Controllers;

/// <summary>
/// Controlador per gestionar els alumnes del sistema.
/// </summary>
[Authorize]
public class StudentsController : BaseController
{
    private readonly IStudentService _studentService;
    private readonly ISchoolService _schoolService;
    private readonly IUserService _userService;

    public StudentsController(
        IStudentService studentService, 
        ISchoolService schoolService,
        IUserService userService,
        ILogger<StudentsController> logger) : base(logger)
    {
        _studentService = studentService;
        _schoolService = schoolService;
        _userService = userService;
    }
    
    /// <summary>
    /// Mostra el llistat de tots els alumnes.
    /// </summary>
    public async Task<IActionResult> Index()
    {
        try
        {
            var students = await _studentService.GetAllStudentsAsync();
            var viewModels = students.Select(s => new StudentViewModel
            {
                Id = (int)s.Id,
                FirstName = s.User?.FirstName ?? "",
                LastName = s.User?.LastName ?? "",
                Email = s.User?.Email ?? "",
                BirthDate = s.User?.BirthDate.HasValue == true ? s.User.BirthDate.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
                SchoolName = s.School?.Name ?? "Sense escola"
            });
            
            var schools = await _schoolService.GetAllSchoolsAsync();
            ViewBag.Schools = schools.Select(s => new SchoolViewModel
            {
                Id = (int)s.Id,
                Name = s.Name
            }).ToList();
            
            return View(viewModels);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error obtenint llista d'alumnes");
            SetErrorMessage("Error carregant els alumnes. Si us plau, intenta-ho de nou.");
            return View(new List<StudentViewModel>());
        }
    }
    
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            
            var viewModel = new StudentViewModel
            {
                Id = (int)student.Id,
                FirstName = student.User?.FirstName ?? "",
                LastName = student.User?.LastName ?? "",
                Email = student.User?.Email ?? "",
                BirthDate = student.User?.BirthDate.HasValue == true ? student.User.BirthDate.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
                SchoolId = (int)student.SchoolId,
                SchoolName = student.School?.Name ?? "Sense escola"
            };
            
            var schools = await _schoolService.GetAllSchoolsAsync();
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

            // 1. Comprova si ja existeix un usuari amb aquest email
            var existingUser = await _userService.GetUserByEmailAsync(model.Email);
            int userId;
            if (existingUser != null)
            {
                // Ja existeix, agafa el seu id
                userId = (int)existingUser.Id;
            }
            else
            {
                // No existeix, crea el User
                var user = new Domain.Entities.User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    BirthDate = model.BirthDate.HasValue ? DateOnly.FromDateTime(model.BirthDate.Value) : null
                };
                var createdUser = await _userService.CreateUserAsync(user, "user123");
                userId = (int)createdUser.Id;
            }

            // 2. Crear el Student associat al User (sigui nou o existent)
            var student = new Domain.Entities.Student
            {
                SchoolId = model.SchoolId,
                UserId = userId
            };
            await _studentService.CreateStudentAsync(student);

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
            var student = await _studentService.GetStudentByIdAsync(id);
            
            var viewModel = new StudentViewModel
            {
                Id = (int)student.Id,
                FirstName = student.User?.FirstName ?? "",
                LastName = student.User?.LastName ?? "",
                Email = student.User?.Email ?? "",
                BirthDate = student.User?.BirthDate.HasValue == true ? student.User.BirthDate.Value.ToDateTime(TimeOnly.MinValue) : null,
                SchoolId = (int)student.SchoolId,
                SchoolName = student.School?.Name ?? ""
            };
            
            var schools = await _schoolService.GetAllSchoolsAsync();
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
                return Redirect("/Students");
            }

            var student = await _studentService.GetStudentByIdAsync(model.Id);
            
            // Actualitzar dades del User associat
            if (student.UserId.HasValue && student.User != null)
            {
                student.User.FirstName = model.FirstName;
                student.User.LastName = model.LastName;
                student.User.Email = model.Email;
                student.User.BirthDate = model.BirthDate.HasValue ? DateOnly.FromDateTime(model.BirthDate.Value) : null;
                
                await _userService.UpdateUserAsync(student.User);
            }
            
            // Actualitzar escola del Student
            student.SchoolId = model.SchoolId;
            await _studentService.UpdateStudentAsync(student);
            
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
            await _studentService.DeleteStudentAsync(id);
            
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
