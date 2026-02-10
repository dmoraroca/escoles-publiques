using Microsoft.AspNetCore.Mvc;
using Web.Services.Search;

namespace Web.ViewComponents;

/// <summary>
/// ViewComponent for displaying search results in the UI.
/// </summary>
public class SearchResultsViewComponent : ViewComponent
{
    private readonly ISearchResultsBuilder _searchResultsBuilder;

    public SearchResultsViewComponent(
        ISearchResultsBuilder searchResultsBuilder)
    {
        _searchResultsBuilder = searchResultsBuilder;
    }

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
