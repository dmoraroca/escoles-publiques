using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.ViewComponents;

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
        model.Schools = allSchools
            .Where(s => 
                (searchTerms.Count == 0 || searchTerms.Any(term =>
                    s.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    s.Code.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    (s.City != null && s.City.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                    (s.Scope != null && s.Scope.Contains(term, StringComparison.OrdinalIgnoreCase)))) &&
                (string.IsNullOrWhiteSpace(scopeName) || 
                 (s.Scope != null && s.Scope.Equals(scopeName, StringComparison.OrdinalIgnoreCase))))
            .Take(10)
            .Select(s => new SchoolResultViewModel
            {
                Id = s.Id,
                Name = s.Name,
                Code = s.Code,
                City = s.City,
                Scope = s.Scope
            })
            .ToList();

        // Cerca en Alumnes
        var allStudents = await _studentService.GetAllStudentsAsync();
        model.Students = allStudents
            .Where(s => 
                searchTerms.Count > 0 && searchTerms.Any(term =>
                    s.FirstName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    s.LastName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    (s.Email != null && s.Email.Contains(term, StringComparison.OrdinalIgnoreCase))))
            .Take(10)
            .Select(s => new StudentResultViewModel
            {
                Id = s.Id,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Email = s.Email ?? string.Empty,
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
                    (e.Student != null && (e.Student.FirstName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                                          e.Student.LastName.Contains(term, StringComparison.OrdinalIgnoreCase))) ||
                    (e.Student?.School != null && e.Student.School.Name.Contains(term, StringComparison.OrdinalIgnoreCase))))
            .Take(10)
            .Select(e => new EnrollmentResultViewModel
            {
                Id = e.Id,
                StudentName = e.Student != null ? $"{e.Student.FirstName} {e.Student.LastName}" : "Desconegut",
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
                    (f.Enrollment?.Student != null &&
                     (f.Enrollment.Student.FirstName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                      f.Enrollment.Student.LastName.Contains(term, StringComparison.OrdinalIgnoreCase))) ||
                    f.Amount.ToString().Contains(term) ||
                    f.Currency.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    (f.PaymentRef != null && f.PaymentRef.Contains(term, StringComparison.OrdinalIgnoreCase))))
            .Take(10)
            .Select(f => new AnnualFeeResultViewModel
            {
                Id = f.Id,
                StudentName = f.Enrollment?.Student != null 
                    ? $"{f.Enrollment.Student.FirstName} {f.Enrollment.Student.LastName}" 
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
