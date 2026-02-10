using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Web.Services.Api;

namespace Web.ViewComponents;

/// <summary>
/// ViewComponent for displaying search results in the UI.
/// </summary>
public class SearchResultsViewComponent : ViewComponent
{
    private readonly ISchoolsApiClient _schoolApi;
    private readonly IScopesApiClient _scopesApi;
    private readonly IStudentsApiClient _studentsApi;
    private readonly IEnrollmentsApiClient _enrollmentsApi;
    private readonly IAnnualFeesApiClient _annualFeesApi;

    public SearchResultsViewComponent(
        ISchoolsApiClient schoolApi,
        IScopesApiClient scopesApi,
        IStudentsApiClient studentsApi,
        IEnrollmentsApiClient enrollmentsApi,
        IAnnualFeesApiClient annualFeesApi)
    {
        _schoolApi = schoolApi;
        _scopesApi = scopesApi;
        _studentsApi = studentsApi;
        _enrollmentsApi = enrollmentsApi;
        _annualFeesApi = annualFeesApi;
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

        // Carrega àmbits per resoldre nom -> id i id -> nom (normalitzat)
        var scopes = (await _scopesApi.GetAllAsync()).ToList();
        var scopeById = scopes.ToDictionary(s => s.Id, s => s.Name);
        var scopeIdByName = scopes
            .Select(s => new { Id = s.Id, Name = NormalizeKey(s.Name) })
            .Where(s => !string.IsNullOrWhiteSpace(s.Name))
            .GroupBy(s => s.Name, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First().Id, StringComparer.OrdinalIgnoreCase);

        var normalizedScopeName = NormalizeKey(scopeName);
        long? scopeFilterId = null;
        if (!string.IsNullOrWhiteSpace(normalizedScopeName) && scopeIdByName.TryGetValue(normalizedScopeName, out var foundScopeId))
        {
            scopeFilterId = foundScopeId;
        }

        // Cerca en Escoles
        var allSchools = (await _schoolApi.GetAllAsync()).ToList();
        model.Schools = allSchools
            .Where(s =>
                (searchTerms.Count == 0 || searchTerms.Any(term =>
                    s.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    s.Code.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    (s.City != null && s.City.Contains(term, StringComparison.OrdinalIgnoreCase)))) &&
                (
                    scopeFilterId == null
                        ? (string.IsNullOrWhiteSpace(normalizedScopeName) ||
                           (s.ScopeId.HasValue &&
                            scopeById.TryGetValue(s.ScopeId.Value, out var scopeLabel) &&
                            NormalizeKey(scopeLabel) == normalizedScopeName))
                        : (s.ScopeId.HasValue && s.ScopeId.Value == scopeFilterId.Value)
                ))
            .Take(10)
            .Select(s => new SchoolResultViewModel
            {
                Id = s.Id,
                Name = s.Name,
                Code = s.Code,
                City = s.City,
                ScopeId = s.ScopeId,
                ScopeName = s.ScopeId.HasValue && scopeById.TryGetValue(s.ScopeId.Value, out var scopeLabel) ? scopeLabel : null,
                Scope = s.ScopeId.HasValue && scopeById.TryGetValue(s.ScopeId.Value, out var scopeLabelLegacy) ? scopeLabelLegacy : null
            })
            .ToList();

        // Cerca en Alumnes
        var allStudents = await _studentsApi.GetAllAsync();
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
                Email = s.Email,
                SchoolName = s.SchoolName
            })
            .ToList();

        // Cerca en Inscripcions
        var allEnrollments = await _enrollmentsApi.GetAllAsync();
        model.Enrollments = allEnrollments
            .Where(e =>
                searchTerms.Count > 0 && searchTerms.Any(term =>
                    e.AcademicYear.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    e.StudentName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    e.SchoolName.Contains(term, StringComparison.OrdinalIgnoreCase)))
            .Take(10)
            .Select(e => new EnrollmentResultViewModel
            {
                Id = e.Id,
                StudentName = e.StudentName,
                SchoolName = e.SchoolName,
                AcademicYear = e.AcademicYear,
                EnrollmentDate = e.EnrolledAt
            })
            .ToList();

        // Cerca en Quotes
        var allFees = await _annualFeesApi.GetAllAsync();
        model.AnnualFees = allFees
            .Where(f =>
                searchTerms.Count > 0 && searchTerms.Any(term =>
                    f.StudentName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    f.Amount.ToString().Contains(term) ||
                    f.Currency.Contains(term, StringComparison.OrdinalIgnoreCase)))
            .Take(10)
            .Select(f => new AnnualFeeResultViewModel
            {
                Id = f.Id,
                StudentName = f.StudentName,
                Amount = f.Amount,
                Currency = f.Currency,
                DueDate = f.DueDate,
                IsPaid = f.PaidAt.HasValue
            })
            .ToList();

        return View(model);
    }

    private static string NormalizeKey(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;
        var sb = new System.Text.StringBuilder(value.Length);
        var prevWasSpace = false;
        foreach (var ch in value)
        {
            if (char.IsWhiteSpace(ch))
            {
                if (!prevWasSpace)
                {
                    sb.Append(' ');
                    prevWasSpace = true;
                }
            }
            else
            {
                sb.Append(ch);
                prevWasSpace = false;
            }
        }
        return sb.ToString().Trim().ToLowerInvariant();
    }
}
