using Application.Interfaces.Search;
using Web.Models;

namespace Web.Services.Search;

public class SearchResultsBuilder : ISearchResultsBuilder
{
    private readonly ISearchResultsQuery _query;

    public SearchResultsBuilder(ISearchResultsQuery query)
    {
        _query = query;
    }

    public async Task<SearchResultsViewModel> BuildAsync(string? searchQuery, string? scopeName)
    {
        var dto = await _query.ExecuteAsync(searchQuery, scopeName);

        return new SearchResultsViewModel
        {
            SearchQuery = dto.SearchQuery,
            ScopeName = dto.ScopeName,
            Schools = dto.Schools.Select(s => new SchoolResultViewModel
            {
                Id = s.Id,
                Name = s.Name,
                Code = s.Code,
                City = s.City,
                ScopeId = s.ScopeId,
                ScopeName = s.ScopeName,
                Scope = s.Scope
            }).ToList(),
            Students = dto.Students.Select(s => new StudentResultViewModel
            {
                Id = s.Id,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Email = s.Email ?? string.Empty,
                SchoolName = s.SchoolName
            }).ToList(),
            Enrollments = dto.Enrollments.Select(e => new EnrollmentResultViewModel
            {
                Id = e.Id,
                StudentName = e.StudentName,
                SchoolName = e.SchoolName,
                AcademicYear = e.AcademicYear,
                EnrollmentDate = e.EnrollmentDate
            }).ToList(),
            AnnualFees = dto.AnnualFees.Select(f => new AnnualFeeResultViewModel
            {
                Id = f.Id,
                StudentName = f.StudentName,
                Amount = f.Amount,
                Currency = f.Currency,
                DueDate = f.DueDate,
                IsPaid = f.IsPaid
            }).ToList()
        };
    }
}
