using Web.Models;

namespace Web.Services.Search;

public interface ISearchResultsBuilder
{
    Task<SearchResultsViewModel> BuildAsync(string? searchQuery, string? scopeName);
}
