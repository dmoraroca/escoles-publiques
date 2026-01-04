using Application.Interfaces;
using Domain.DomainExceptions;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers;

public class EnrollmentsController : BaseController
{
    private readonly IEnrollmentService _enrollmentService;

    public EnrollmentsController(IEnrollmentService enrollmentService, ILogger<EnrollmentsController> logger) : base(logger)
    {
        _enrollmentService = enrollmentService;
    }
    
    public async Task<IActionResult> Index()
    {
        try
        {
            var enrollments = await _enrollmentService.GetAllEnrollmentsAsync();
            var viewModels = enrollments.Select(e => new EnrollmentViewModel
            {
                Id = (int)e.Id,
                StudentName = e.Student != null ? $"{e.Student.FirstName} {e.Student.LastName}" : "Alumne desconegut",
                AcademicYear = e.AcademicYear,
                Status = e.Status,
                EnrolledAt = e.EnrolledAt
            });
            
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
                StudentName = enrollment.Student != null ? $"{enrollment.Student.FirstName} {enrollment.Student.LastName}" : "Alumne desconegut",
                AcademicYear = enrollment.AcademicYear,
                CourseName = enrollment.CourseName,
                Status = enrollment.Status,
                EnrolledAt = enrollment.EnrolledAt
            };
            
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
    
    public IActionResult Create()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(int studentId, string academicYear, string status)
    {
        // TODO: Crear nova inscripció
        return RedirectToAction(nameof(Index));
    }
}
