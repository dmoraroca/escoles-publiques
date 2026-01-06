using System.Security.Claims;
using Application.Interfaces;
using Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

/// <summary>
/// Controlador per gestionar la informació i el dashboard de l'usuari.
/// </summary>
[Authorize]
public class UserController : BaseController
{
    private readonly IStudentService _studentService;
    private readonly IEnrollmentService _enrollmentService;
    private readonly IAnnualFeeService _annualFeeService;
    private readonly IUserRepository _userRepository;

    public UserController(
        ILogger<UserController> logger,
        IStudentService studentService,
        IEnrollmentService enrollmentService,
        IAnnualFeeService annualFeeService,
        IUserRepository userRepository) : base(logger)
    {
        _studentService = studentService;
        _enrollmentService = enrollmentService;
        _annualFeeService = annualFeeService;
        _userRepository = userRepository;
    }

    /// <summary>
    /// Mostra el dashboard personalitzat de l'usuari amb inscripcions i quotes.
    /// </summary>
    public async Task<IActionResult> Dashboard()
    {
        try
        {
            // Obtenir l'ID de l'usuari des del claim
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Logger.LogInformation("=== DASHBOARD ===");
            Logger.LogInformation("UserIdClaim: {UserIdClaim}", userIdClaim);
            
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            Logger.LogInformation("UserId parsed: {UserId}", userId);

            // Verificar que l'usuari és USER, no ADM
            var role = User.FindFirstValue("Role");
            Logger.LogInformation("Role: {Role}", role);
            
            if (role == "ADM")
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                Logger.LogWarning("User not found with ID: {UserId}", userId);
                return RedirectToAction("Login", "Auth");
            }

            Logger.LogInformation("User found: {FirstName} {LastName}", user.FirstName, user.LastName);

            // Obtenir l'alumne relacionat amb aquest usuari
            var students = await _studentService.GetAllStudentsAsync();
            Logger.LogInformation("Total students in DB: {Count}", students.Count());
            
            var student = students.FirstOrDefault(s => s.UserId == userId);

            if (student == null)
            {
                Logger.LogWarning("No student found for user ID: {UserId}", userId);
                ViewBag.Message = "No s'ha trobat cap alumne associat al teu usuari";
                ViewBag.User = user;
                ViewBag.Enrollments = new List<dynamic>();
                ViewBag.Fees = new List<dynamic>();
                return View();
            }

            Logger.LogInformation("Student found with ID: {StudentId}", student.Id);

            // Obtenir les inscripcions de l'alumne
            var allEnrollments = await _enrollmentService.GetAllEnrollmentsAsync();
            Logger.LogInformation("Total enrollments in DB: {Count}", allEnrollments.Count());
            
            var userEnrollments = allEnrollments
                .Where(e => e.StudentId == student.Id)
                .Select(e => new Web.Models.EnrollmentViewModel {
                    Id = (int)e.Id,
                    StudentId = (int)e.StudentId,
                    StudentName = student.User?.FirstName + " " + student.User?.LastName,
                    AcademicYear = e.AcademicYear ?? "",
                    CourseName = e.CourseName ?? "",
                    Status = e.Status ?? "",
                    EnrolledAt = e.EnrolledAt,
                    CreatedAt = student.CreatedAt
                })
                .ToList();
            Logger.LogInformation("Enrollments for student {StudentId}: {Count}", student.Id, userEnrollments.Count);

            // Obtenir les quotes de les inscripcions
            var allFees = await _annualFeeService.GetAllAnnualFeesAsync();
            var userFees = allFees.Where(f => f.Enrollment != null && f.Enrollment.StudentId == student.Id).ToList();
            Logger.LogInformation("Fees for student {StudentId}: {Count}", student.Id, userFees.Count);

            ViewBag.User = user;
            ViewBag.Student = student;
            ViewBag.Enrollments = userEnrollments;
            ViewBag.Fees = userFees;

            return View();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error carregant el dashboard de l'usuari");
            ViewBag.Message = "S'ha produït un error carregant el tauler d'usuari.";
            ViewBag.User = null;
            ViewBag.Enrollments = new List<dynamic>();
            ViewBag.Fees = new List<dynamic>();
            return View();
        }
    }
}
