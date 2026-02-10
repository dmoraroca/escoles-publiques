using System.Text;
using Application.DTOs;
using Application.Interfaces.Search;

namespace Application.UseCases.Queries.SearchResults;

public class SearchResultsQuery : ISearchResultsQuery
{
    private readonly ISchoolSearchSource _schoolSource;
    private readonly IScopeLookupSource _scopeSource;
    private readonly IStudentSearchSource _studentSource;
    private readonly IEnrollmentSearchSource _enrollmentSource;
    private readonly IAnnualFeeSearchSource _annualFeeSource;

    public SearchResultsQuery(
        ISchoolSearchSource schoolSource,
        IScopeLookupSource scopeSource,
        IStudentSearchSource studentSource,
        IEnrollmentSearchSource enrollmentSource,
        IAnnualFeeSearchSource annualFeeSource)
    {
        _schoolSource = schoolSource;
        _scopeSource = scopeSource;
        _studentSource = studentSource;
        _enrollmentSource = enrollmentSource;
        _annualFeeSource = annualFeeSource;
    }

    public async Task<SearchResultsDto> ExecuteAsync(string? searchQuery, string? scopeName)
    {
        var model = new SearchResultsDto
        {
            SearchQuery = searchQuery,
            ScopeName = scopeName
        };

        var searchTerms = ParseSearchTerms(searchQuery);

        var scopes = (await _scopeSource.GetAllAsync()).ToList();
        var scopeById = scopes.ToDictionary(s => s.Id, s => s.Name);
        var scopeIdByName = scopes
            .Select(s => new { s.Id, Name = NormalizeKey(s.Name) })
            .Where(s => !string.IsNullOrWhiteSpace(s.Name))
            .GroupBy(s => s.Name, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First().Id, StringComparer.OrdinalIgnoreCase);

        var normalizedScopeName = NormalizeKey(scopeName);
        long? scopeFilterId = null;
        if (!string.IsNullOrWhiteSpace(normalizedScopeName) &&
            scopeIdByName.TryGetValue(normalizedScopeName, out var foundScopeId))
        {
            scopeFilterId = foundScopeId;
        }

        var allSchools = (await _schoolSource.GetAllAsync()).ToList();
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
            .Select(s => new SchoolResultDto
            {
                Id = s.Id,
                Name = s.Name,
                Code = s.Code,
                City = s.City,
                ScopeId = s.ScopeId,
                ScopeName = s.ScopeId.HasValue && scopeById.TryGetValue(s.ScopeId.Value, out var scopeLabel)
                    ? scopeLabel
                    : null,
                Scope = s.ScopeId.HasValue && scopeById.TryGetValue(s.ScopeId.Value, out var scopeLabelLegacy)
                    ? scopeLabelLegacy
                    : null
            })
            .ToList();

        var allStudents = await _studentSource.GetAllAsync();
        model.Students = allStudents
            .Where(s =>
                searchTerms.Count > 0 && searchTerms.Any(term =>
                    s.FirstName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    s.LastName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    (!string.IsNullOrWhiteSpace(s.Email) && s.Email.Contains(term, StringComparison.OrdinalIgnoreCase))))
            .Take(10)
            .Select(s => new StudentResultDto
            {
                Id = s.Id,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Email = s.Email,
                SchoolName = s.SchoolName
            })
            .ToList();

        var allEnrollments = await _enrollmentSource.GetAllAsync();
        model.Enrollments = allEnrollments
            .Where(e =>
                searchTerms.Count > 0 && searchTerms.Any(term =>
                    e.AcademicYear.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    e.StudentName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    e.SchoolName.Contains(term, StringComparison.OrdinalIgnoreCase)))
            .Take(10)
            .Select(e => new EnrollmentResultDto
            {
                Id = e.Id,
                StudentName = e.StudentName,
                SchoolName = e.SchoolName,
                AcademicYear = e.AcademicYear,
                EnrollmentDate = e.EnrolledAt
            })
            .ToList();

        var allFees = await _annualFeeSource.GetAllAsync();
        model.AnnualFees = allFees
            .Where(f =>
                searchTerms.Count > 0 && searchTerms.Any(term =>
                    f.StudentName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    f.Amount.ToString().Contains(term) ||
                    f.Currency.Contains(term, StringComparison.OrdinalIgnoreCase)))
            .Take(10)
            .Select(f => new AnnualFeeResultDto
            {
                Id = f.Id,
                StudentName = f.StudentName,
                Amount = f.Amount,
                Currency = f.Currency,
                DueDate = f.DueDate,
                IsPaid = f.PaidAt.HasValue
            })
            .ToList();

        return model;
    }

    private static List<string> ParseSearchTerms(string? searchQuery)
    {
        return string.IsNullOrWhiteSpace(searchQuery)
            ? new List<string>()
            : searchQuery
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim())
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .ToList();
    }

    private static string NormalizeKey(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;
        var sb = new StringBuilder(value.Length);
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
