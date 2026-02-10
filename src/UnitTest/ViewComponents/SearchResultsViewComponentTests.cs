using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Moq;
using Web.Models;
using Web.Services.Search;
using Web.ViewComponents;
using Xunit;

namespace UnitTest.ViewComponents
{
    public class SearchResultsViewComponentTests
    {
        [Fact]
        public async Task InvokeAsync_ReturnsEmptyContent_WhenNoQuery()
        {
            var builder = new Mock<ISearchResultsBuilder>();
            var component = new SearchResultsViewComponent(builder.Object);

            var result = await component.InvokeAsync(null, null);

            var content = Assert.IsType<ContentViewComponentResult>(result);
            Assert.Equal(string.Empty, content.Content);
        }

        [Fact]
        public async Task InvokeAsync_ReturnsView_WhenQueryProvided()
        {
            var builder = new Mock<ISearchResultsBuilder>();
            builder.Setup(b => b.BuildAsync("q", "s")).ReturnsAsync(new SearchResultsViewModel());
            var component = new SearchResultsViewComponent(builder.Object);

            var result = await component.InvokeAsync("q", "s");

            var view = Assert.IsType<ViewViewComponentResult>(result);
            Assert.True(view.ViewData?.Model is SearchResultsViewModel);
        }
    }
}
