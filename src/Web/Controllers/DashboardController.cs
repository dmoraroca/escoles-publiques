using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Web.Services.Api;

namespace Web.Controllers;

/// <summary>
/// Controlador per gestionar la informació i el dashboard de l'usuari.
/// </summary>
[Authorize]
public class DashboardController : BaseController
{
    private readonly IStudentsApiClient _studentsApi;
    private readonly IEnrollmentsApiClient _enrollmentsApi;
    private readonly IAnnualFeesApiClient _annualFeesApi;
    private readonly ISchoolsApiClient _schoolsApi;

    public DashboardController(
        ILogger<DashboardController> logger,
        IStudentsApiClient studentsApi,
        IEnrollmentsApiClient enrollmentsApi,
        IAnnualFeesApiClient annualFeesApi,
        ISchoolsApiClient schoolsApi) : base(logger)
    {
        _studentsApi = studentsApi;
        _enrollmentsApi = enrollmentsApi;
        _annualFeesApi = annualFeesApi;
        _schoolsApi = schoolsApi;
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

            // Obtenir l'alumne relacionat amb aquest usuari via API
            var students = await _studentsApi.GetAllAsync();
            Logger.LogInformation("Total students in API: {Count}", students.Count());
            var student = students.FirstOrDefault(s => s.UserId == userId);

            if (student == null)
            {
                Logger.LogWarning("No student found for user ID: {UserId}", userId);
                ViewBag.Message = "No s'ha trobat cap alumne associat al teu usuari";
                var email = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
                ViewBag.User = new UserViewModel
                {
                    Id = (int)userId,
                    Email = email,
                    FirstName = email
                };
                ViewBag.Student = null;
                ViewBag.Enrollments = new List<EnrollmentViewModel>();
                ViewBag.Fees = new List<AnnualFeeViewModel>();
                return View();
            }

            Logger.LogInformation("Student found with ID: {StudentId}", student.Id);

            var studentVm = new StudentViewModel
            {
                Id = (int)student.Id,
                UserId = student.UserId,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Email = student.Email,
                BirthDate = student.BirthDate.HasValue ? student.BirthDate.Value.ToDateTime(TimeOnly.MinValue) : null,
                SchoolId = (int)student.SchoolId,
                SchoolName = student.SchoolName ?? string.Empty
            };

            var userVm = new UserViewModel
            {
                Id = (int)userId,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Email = student.Email,
                StudentId = (int?)student.Id
            };

            // Inscripcions via API
            var allEnrollments = await _enrollmentsApi.GetAllAsync();
            Logger.LogInformation("Total enrollments in API: {Count}", allEnrollments.Count());

            var userEnrollments = allEnrollments
                .Where(e => e.StudentId == student.Id)
                .Select(e => new EnrollmentViewModel
                {
                    Id = (int)e.Id,
                    StudentId = (int)e.StudentId,
                    StudentName = e.StudentName,
                    AcademicYear = e.AcademicYear ?? string.Empty,
                    CourseName = e.CourseName ?? string.Empty,
                    Status = e.Status ?? string.Empty,
                    EnrolledAt = e.EnrolledAt,
                    SchoolId = (int)e.SchoolId,
                    SchoolName = e.SchoolName ?? string.Empty
                })
                .ToList();
            Logger.LogInformation("Enrollments for student {StudentId}: {Count}", student.Id, userEnrollments.Count);

            // Quotes via API
            var allFees = await _annualFeesApi.GetAllAsync();
            var userFees = allFees
                .Where(f => f.EnrollmentId != 0 && userEnrollments.Any(e => e.Id == (int)f.EnrollmentId))
                .Select(f => new AnnualFeeViewModel
                {
                    Id = (int)f.Id,
                    EnrollmentId = (int)f.EnrollmentId,
                    EnrollmentInfo = f.EnrollmentInfo,
                    Amount = f.Amount,
                    Currency = f.Currency,
                    DueDate = f.DueDate,
                    PaidAt = f.PaidAt,
                    PaymentRef = f.PaymentRef,
                    SchoolId = (int)(f.SchoolId ?? 0),
                    SchoolName = f.SchoolName ?? string.Empty,
                    AcademicYear = f.AcademicYear ?? string.Empty
                })
                .ToList();
            Logger.LogInformation("Fees for student {StudentId}: {Count}", student.Id, userFees.Count);

            ViewBag.User = userVm;
            ViewBag.Student = studentVm;
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
