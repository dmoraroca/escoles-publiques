using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Moq;
using Web.Services.Api;
using Web.ViewComponents;
using Xunit;

namespace UnitTest.ViewComponents
{
    public class ScopesViewComponentTests
    {
        [Fact]
        public async Task InvokeAsync_ReturnsViewWithScopes()
        {
            var scopesApi = new Mock<IScopesApiClient>();
            scopesApi.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<ApiScope> { new ApiScope(1, " Primary ") });

            var component = new ScopesViewComponent(scopesApi.Object);
            var result = await component.InvokeAsync();

            Assert.IsType<ViewViewComponentResult>(result);
        }
    }
}
