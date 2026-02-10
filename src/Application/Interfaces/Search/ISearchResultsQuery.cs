using Application.DTOs;

namespace Application.Interfaces.Search;

public interface ISearchResultsQuery
{
    Task<SearchResultsDto> ExecuteAsync(string? searchQuery, string? scopeName);
}
