using Application.Interfaces;
using Domain.DomainExceptions;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

using Microsoft.AspNetCore.Authorization;
namespace Web.Controllers;

/// <summary>
/// Controlador per gestionar les inscripcions dels alumnes.
/// </summary>
[Authorize]
public class EnrollmentsController : BaseController
{
    private readonly IEnrollmentService _enrollmentService;
    private readonly IStudentService _studentService;

    public EnrollmentsController(
        IEnrollmentService enrollmentService, 
        IStudentService studentService,
        ILogger<EnrollmentsController> logger) : base(logger)
    {
        _enrollmentService = enrollmentService;
        _studentService = studentService;
    }
    
    /// <summary>
    /// Mostra el llistat de totes les inscripcions.
    /// </summary>
    public async Task<IActionResult> Index()
    {
        try
        {
            var enrollments = await _enrollmentService.GetAllEnrollmentsAsync();
            var viewModels = enrollments.Select(e => new EnrollmentViewModel
            {
                Id = (int)e.Id,
                StudentName = e.Student?.User != null ? $"{e.Student.User.FirstName} {e.Student.User.LastName}" : "Alumne desconegut",
                AcademicYear = e.AcademicYear,
                Status = e.Status,
                EnrolledAt = e.EnrolledAt
            });
            
            var students = await _studentService.GetAllStudentsAsync();
            ViewBag.Students = students.Select(s => new StudentViewModel
            {
                Id = (int)s.Id,
                FirstName = s.User?.FirstName ?? "",
                LastName = s.User?.LastName ?? ""
            }).ToList();
            
            return View(viewModels);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error obtenint llista d'inscripcions");
            SetErrorMessage("Error carregant les inscripcions. Si us plau, intenta-ho de nou.");
            return View(new List<EnrollmentViewModel>());
        }
    }
    
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var enrollment = await _enrollmentService.GetEnrollmentByIdAsync(id);
            
            var viewModel = new EnrollmentViewModel
            {
                Id = (int)enrollment.Id,
                StudentId = (int)enrollment.StudentId,
                StudentName = enrollment.Student?.User != null ? $"{enrollment.Student.User.FirstName} {enrollment.Student.User.LastName}" : "Alumne desconegut",
                AcademicYear = enrollment.AcademicYear,
                CourseName = enrollment.CourseName,
                Status = enrollment.Status,
                EnrolledAt = enrollment.EnrolledAt
            };
            
            var students = await _studentService.GetAllStudentsAsync();
            ViewBag.Students = students.Select(s => new StudentViewModel
            {
                Id = (int)s.Id,
                FirstName = s.User?.FirstName ?? "",
                LastName = s.User?.LastName ?? ""
            }).ToList();
            
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
            Logger.LogError(ex, "Error obtenint detalls de la inscripció {Id}", id);
            SetErrorMessage("Error carregant els detalls de la inscripció.");
            return RedirectToAction(nameof(Index));
        }
    }
    
    public async Task<IActionResult> Create()
    {
        var students = await _studentService.GetAllStudentsAsync();
        ViewBag.Students = students.Select(s => new StudentViewModel
        {
            Id = (int)s.Id,
            FirstName = s.User?.FirstName ?? "",
            LastName = s.User?.LastName ?? ""
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

            var enrollment = new Domain.Entities.Enrollment
            {
                StudentId = model.StudentId,
                AcademicYear = model.AcademicYear,
                CourseName = model.CourseName,
                Status = model.Status
            };
            
            await _enrollmentService.CreateEnrollmentAsync(enrollment);
            
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
            var enrollment = await _enrollmentService.GetEnrollmentByIdAsync(id);
            
            var viewModel = new EnrollmentViewModel
            {
                Id = (int)enrollment.Id,
                StudentId = (int)enrollment.StudentId,
                StudentName = enrollment.Student?.User != null ? $"{enrollment.Student.User.FirstName} {enrollment.Student.User.LastName}" : "Alumne desconegut",
                AcademicYear = enrollment.AcademicYear,
                CourseName = enrollment.CourseName,
                Status = enrollment.Status,
                EnrolledAt = enrollment.EnrolledAt
            };
            
            // Carregar estudiants per al dropdown
            var students = await _studentService.GetAllStudentsAsync();
            ViewBag.Students = students.Select(s => new StudentViewModel
            {
                Id = (int)s.Id,
                FirstName = s.User?.FirstName ?? "",
                LastName = s.User?.LastName ?? ""
            }).ToList();
            
            return View(viewModel);
        }
        catch (NotFoundException ex)
        {
            Logger.LogWarning(ex, "Inscripció amb Id {Id} no trobada per editar", id);
            SetErrorMessage($"Inscripció amb ID {id} no trobada.");
            return RedirectToAction(nameof(Index));
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
                // Recarregar estudiants si hi ha errors
                var students = await _studentService.GetAllStudentsAsync();
                ViewBag.Students = students.Select(s => new StudentViewModel
                {
                    Id = (int)s.Id,
                    FirstName = s.User?.FirstName ?? "",
                    LastName = s.User?.LastName ?? ""
                }).ToList();
                
                SetErrorMessage("Si us plau, omple tots els camps obligatoris correctament.");
                return View(model);
            }

            var enrollment = await _enrollmentService.GetEnrollmentByIdAsync(model.Id);
            
            enrollment.StudentId = model.StudentId;
            enrollment.AcademicYear = model.AcademicYear;
            enrollment.CourseName = model.CourseName;
            enrollment.Status = model.Status;

            await _enrollmentService.UpdateEnrollmentAsync(enrollment);
            
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
            await _enrollmentService.DeleteEnrollmentAsync(id);
            
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
