using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.ViewComponents;

/// <summary>
/// ViewComponent for displaying search results in the UI.
/// </summary>
public class SearchResultsViewComponent : ViewComponent
{
    private readonly ISchoolService _schoolService;
    private readonly IStudentService _studentService;
    private readonly IEnrollmentService _enrollmentService;
    private readonly IAnnualFeeService _annualFeeService;

    public SearchResultsViewComponent(
        ISchoolService schoolService,
        IStudentService studentService,
        IEnrollmentService enrollmentService,
        IAnnualFeeService annualFeeService)
    {
        _schoolService = schoolService;
        _studentService = studentService;
        _enrollmentService = enrollmentService;
        _annualFeeService = annualFeeService;
    }

    public async Task<IViewComponentResult> InvokeAsync(string? searchQuery, string? scopeName)
    {
        var model = new SearchResultsViewModel
        {
            SearchQuery = searchQuery,
            ScopeName = scopeName
        };

        if (string.IsNullOrWhiteSpace(searchQuery) && string.IsNullOrWhiteSpace(scopeName))
        {
            return Content(string.Empty);
        }

        // Dividir la cerca per comes i netejar espais
        var searchTerms = string.IsNullOrWhiteSpace(searchQuery) 
            ? new List<string>() 
            : searchQuery.Split(',', StringSplitOptions.RemoveEmptyEntries)
                         .Select(t => t.Trim())
                         .Where(t => !string.IsNullOrWhiteSpace(t))
                         .ToList();

        // Cerca en Escoles
        var allSchools = await _schoolService.GetAllSchoolsAsync();
        var allScopes = allSchools.Select(s => s.ScopeId).Distinct().Where(id => id.HasValue).Select(id => id.Value).ToList();
        // Aquí hauries d'obtenir els noms dels àmbits segons la teva implementació de repositori
        // Per simplicitat, suposarem que no es filtra per nom d'àmbit, només per id
        model.Schools = allSchools
            .Where(s => 
                (searchTerms.Count == 0 || searchTerms.Any(term =>
                    s.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    s.Code.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    (s.City != null && s.City.Contains(term, StringComparison.OrdinalIgnoreCase)))) &&
                (string.IsNullOrWhiteSpace(scopeName) || 
                 (s.ScopeId.HasValue && s.ScopeId.ToString() == scopeName)))
            .Take(10)
            .Select(s => new SchoolResultViewModel
            {
                Id = s.Id,
                Name = s.Name,
                Code = s.Code,
                City = s.City,
                ScopeId = s.ScopeId,
                ScopeName = null // Omple amb el nom real si tens accés aquí
            })
            .ToList();

        // Cerca en Alumnes
        var allStudents = await _studentService.GetAllStudentsAsync();
        model.Students = allStudents
            .Where(s => 
                s.User != null && searchTerms.Count > 0 && searchTerms.Any(term =>
                    s.User.FirstName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    s.User.LastName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    (s.User.Email != null && s.User.Email.Contains(term, StringComparison.OrdinalIgnoreCase))))
            .Take(10)
            .Select(s => new StudentResultViewModel
            {
                Id = s.Id,
                FirstName = s.User?.FirstName ?? string.Empty,
                LastName = s.User?.LastName ?? string.Empty,
                Email = s.User?.Email ?? string.Empty,
                SchoolName = s.School?.Name
            })
            .ToList();

        // Cerca en Inscripcions
        var allEnrollments = await _enrollmentService.GetAllEnrollmentsAsync();
        model.Enrollments = allEnrollments
            .Where(e => 
                searchTerms.Count > 0 && searchTerms.Any(term =>
                    e.AcademicYear.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    (e.CourseName != null && e.CourseName.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                    e.Status.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    (e.Student?.User != null && (e.Student.User.FirstName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                                          e.Student.User.LastName.Contains(term, StringComparison.OrdinalIgnoreCase))) ||
                    (e.Student?.School != null && e.Student.School.Name.Contains(term, StringComparison.OrdinalIgnoreCase))))
            .Take(10)
            .Select(e => new EnrollmentResultViewModel
            {
                Id = e.Id,
                StudentName = e.Student?.User != null ? $"{e.Student.User.FirstName} {e.Student.User.LastName}" : "Desconegut",
                SchoolName = e.Student?.School?.Name ?? "Desconeguda",
                AcademicYear = e.AcademicYear,
                EnrollmentDate = e.EnrolledAt
            })
            .ToList();

        // Cerca en Quotes
        var allFees = await _annualFeeService.GetAllAnnualFeesAsync();
        model.AnnualFees = allFees
            .Where(f => 
                searchTerms.Count > 0 && searchTerms.Any(term =>
                    (f.Enrollment?.Student?.User != null &&
                     (f.Enrollment.Student.User.FirstName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                      f.Enrollment.Student.User.LastName.Contains(term, StringComparison.OrdinalIgnoreCase))) ||
                    f.Amount.ToString().Contains(term) ||
                    f.Currency.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    (f.PaymentRef != null && f.PaymentRef.Contains(term, StringComparison.OrdinalIgnoreCase))))
            .Take(10)
            .Select(f => new AnnualFeeResultViewModel
            {
                Id = f.Id,
                StudentName = f.Enrollment?.Student?.User != null 
                    ? $"{f.Enrollment.Student.User.FirstName} {f.Enrollment.Student.User.LastName}" 
                    : "Desconegut",
                Amount = f.Amount,
                Currency = f.Currency,
                DueDate = f.DueDate,
                IsPaid = f.PaidAt.HasValue
            })
            .ToList();

        return View(model);
    }
}
