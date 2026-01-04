using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers;

public class EnrollmentsController : BaseController
{
    public EnrollmentsController(ILogger<EnrollmentsController> logger) : base(logger)
    {
    }
    
    public IActionResult Index()
    {
        var enrollments = new List<EnrollmentViewModel>
        {
            new EnrollmentViewModel { Id = 1, StudentName = "Marc García López", AcademicYear = "2025-2026", Status = "Active", EnrolledAt = new DateTime(2025, 9, 1) },
            new EnrollmentViewModel { Id = 2, StudentName = "Laura Martínez Rodríguez", AcademicYear = "2025-2026", Status = "Active", EnrolledAt = new DateTime(2025, 9, 1) },
            new EnrollmentViewModel { Id = 3, StudentName = "Pau Sánchez Pujol", AcademicYear = "2024-2025", Status = "Finished", EnrolledAt = new DateTime(2024, 9, 1) },
            new EnrollmentViewModel { Id = 4, StudentName = "Anna Ferrer Vidal", AcademicYear = "2025-2026", Status = "Active", EnrolledAt = new DateTime(2025, 9, 5) },
            new EnrollmentViewModel { Id = 5, StudentName = "Joan Roca Casals", AcademicYear = "2025-2026", Status = "Cancelled", EnrolledAt = new DateTime(2025, 9, 1) }
        };
        
        return View(enrollments);
    }
    
    public IActionResult Details(int id)
    {
        // TODO: Carregar detalls d'una inscripció
        return View();
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
