using Microsoft.AspNetCore.Mvc;
using Web.Services.Search;

namespace Web.ViewComponents;
/// <summary>
/// Encapsulates the functional responsibility of search results view component within the application architecture.
/// </summary>
public class SearchResultsViewComponent : ViewComponent
{
    private readonly ISearchResultsBuilder _searchResultsBuilder;

    public SearchResultsViewComponent(
        ISearchResultsBuilder searchResultsBuilder)
    {
        _searchResultsBuilder = searchResultsBuilder;
    }
            /// <summary>
            /// Executes middleware logic for the current HTTP request.
            /// </summary>
            public async Task<IViewComponentResult> InvokeAsync(string? searchQuery, string? scopeName)
    {
        if (string.IsNullOrWhiteSpace(searchQuery) && string.IsNullOrWhiteSpace(scopeName))
        {
            return Content(string.Empty);
        }

        var model = await _searchResultsBuilder.BuildAsync(searchQuery, scopeName);
        return View(model);
    }
}
