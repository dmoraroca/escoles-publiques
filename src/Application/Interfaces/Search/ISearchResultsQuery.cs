using Application.DTOs;

namespace Application.Interfaces.Search;
/// <summary>
/// Defines the contract required by i search results query components.
/// </summary>
public interface ISearchResultsQuery
{
    Task<SearchResultsDto> ExecuteAsync(string? searchQuery, string? scopeName);
}
