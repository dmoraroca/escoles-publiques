using Application.Interfaces;
using Domain.DomainExceptions;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers;

public class StudentsController : BaseController
{
    private readonly IStudentService _studentService;
    private readonly ISchoolService _schoolService;

    public StudentsController(
        IStudentService studentService, 
        ISchoolService schoolService,
        ILogger<StudentsController> logger) : base(logger)
    {
        _studentService = studentService;
        _schoolService = schoolService;
    }
    
    public async Task<IActionResult> Index()
    {
        try
        {
            var students = await _studentService.GetAllStudentsAsync();
            var viewModels = students.Select(s => new StudentViewModel
            {
                Id = (int)s.Id,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Email = s.Email ?? "",
                BirthDate = s.BirthDate.HasValue ? s.BirthDate.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
                SchoolName = s.School?.Name ?? "Sense escola"
            });
            
            // Carregar escoles per al dropdown del modal
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
                FirstName = student.FirstName,
                LastName = student.LastName,
                Email = student.Email ?? "",
                BirthDate = student.BirthDate.HasValue ? student.BirthDate.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
                SchoolName = student.School?.Name ?? "Sense escola"
            };
            
            return View(viewModel);
        }
        catch (NotFoundException ex)
        {
            Logger.LogWarning(ex, "Alumne amb Id {Id} no trobat", id);
            SetErrorMessage($"Alumne amb ID {id} no trobat.");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error obtenint detalls de l'alumne {Id}", id);
            SetErrorMessage("Error carregant els detalls de l'alumne.");
            return RedirectToAction(nameof(Index));
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
                SetErrorMessage("Si us plau, omple tots els camps obligatoris correctament.");
                return RedirectToAction(nameof(Index));
            }

            var student = new Domain.Entities.Student
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = string.IsNullOrEmpty(model.Email) ? null : model.Email,
                BirthDate = model.BirthDate.HasValue ? DateOnly.FromDateTime(model.BirthDate.Value) : null,
                SchoolId = model.SchoolId
            };
            
            await _studentService.CreateStudentAsync(student);
            
            SetSuccessMessage($"Alumne {model.FirstName} {model.LastName} creat correctament.");
            return RedirectToAction(nameof(Index));
        }
        catch (NotFoundException ex)
        {
            Logger.LogWarning(ex, "Escola no trobada al crear alumne");
            SetErrorMessage("L'escola seleccionada no existeix.");
            return RedirectToAction(nameof(Index));
        }
        catch (ValidationException ex)
        {
            Logger.LogWarning(ex, "Error de validació al crear alumne");
            SetErrorMessage($"Error de validació: {string.Join(", ", ex.Errors.SelectMany(e => e.Value))}");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creant alumne");
            SetErrorMessage("Error creant l'alumne. Si us plau, intenta-ho de nou.");
            return RedirectToAction(nameof(Index));
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
                FirstName = student.FirstName,
                LastName = student.LastName,
                Email = student.Email ?? "",
                BirthDate = student.BirthDate.HasValue ? student.BirthDate.Value.ToDateTime(TimeOnly.MinValue) : null,
                SchoolId = (int)student.SchoolId,
                SchoolName = student.School?.Name ?? ""
            };
            
            // Carregar escoles per al dropdown
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
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error carregant alumne per editar {Id}", id);
            SetErrorMessage("Error carregant l'alumne.");
            return RedirectToAction(nameof(Index));
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
                // Recarregar escoles si hi ha errors
                var schools = await _schoolService.GetAllSchoolsAsync();
                ViewBag.Schools = schools.Select(s => new SchoolViewModel
                {
                    Id = (int)s.Id,
                    Name = s.Name
                }).ToList();
                
                SetErrorMessage("Si us plau, omple tots els camps obligatoris correctament.");
                return View(model);
            }

            var student = await _studentService.GetStudentByIdAsync(model.Id);
            
            student.FirstName = model.FirstName;
            student.LastName = model.LastName;
            student.Email = string.IsNullOrEmpty(model.Email) ? null : model.Email;
            student.BirthDate = model.BirthDate.HasValue ? DateOnly.FromDateTime(model.BirthDate.Value) : null;
            student.SchoolId = model.SchoolId;

            await _studentService.UpdateStudentAsync(student);
            
            SetSuccessMessage($"Alumne {student.FirstName} {student.LastName} actualitzat correctament.");
            return RedirectToAction(nameof(Details), new { id = model.Id });
        }
        catch (NotFoundException ex)
        {
            Logger.LogWarning(ex, "Alumne o escola no trobats al actualitzar");
            SetErrorMessage("L'alumne o l'escola no existeixen.");
            return RedirectToAction(nameof(Index));
        }
        catch (ValidationException ex)
        {
            Logger.LogWarning(ex, "Error de validació al actualitzar alumne");
            SetErrorMessage($"Error de validació: {string.Join(", ", ex.Errors.SelectMany(e => e.Value))}");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error actualitzant alumne {Id}", model.Id);
            SetErrorMessage("Error al actualitzar l'alumne. Si us plau, intenta-ho de nou.");
            return RedirectToAction(nameof(Index));
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _studentService.DeleteStudentAsync(id);
            
            SetSuccessMessage("Alumne esborrat correctament.");
            return RedirectToAction(nameof(Index));
        }
        catch (NotFoundException ex)
        {
            Logger.LogWarning(ex, "Intent d'esborrar alumne inexistent: {Id}", id);
            SetErrorMessage("L'alumne no existeix.");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error esborrant alumne {Id}", id);
            SetErrorMessage("Error al esborrar l'alumne. Pot tenir matrícules associades.");
            return RedirectToAction(nameof(Index));
        }
    }
}
