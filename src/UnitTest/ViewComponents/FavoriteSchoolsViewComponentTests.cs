using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Moq;
using Web.Services.Api;
using Web.ViewComponents;
using Xunit;

namespace UnitTest.ViewComponents
{
    public class FavoriteSchoolsViewComponentTests
    {
        [Fact]
        public async Task InvokeAsync_ReturnsViewWithFavorites()
        {
            var schoolsApi = new Mock<ISchoolsApiClient>();
            schoolsApi.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<Domain.Entities.School>
            {
                new Domain.Entities.School { Id = 1, Name = "A", Code = "A1", IsFavorite = true, City = "City" },
                new Domain.Entities.School { Id = 2, Name = "B", Code = "B1", IsFavorite = false }
            });

            var component = new FavoriteSchoolsViewComponent(schoolsApi.Object);
            component.Url = Mock.Of<IUrlHelper>(u => u.Action(It.IsAny<UrlActionContext>()) == "/Schools/Details/1");

            var result = await component.InvokeAsync();

            Assert.IsType<ViewViewComponentResult>(result);
        }
    }
}
