using Web.Models;

namespace Web.Services.Search;
/// <summary>
/// Defines the contract required by i search results builder components.
/// </summary>
public interface ISearchResultsBuilder
{
    Task<SearchResultsViewModel> BuildAsync(string? searchQuery, string? scopeName);
}
