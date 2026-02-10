using System;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Web.Models;
using Web.ViewComponents;
using Xunit;

namespace UnitTest.ViewComponents
{
    public class GenericTableViewComponentTests
    {
        [Fact]
        public void Invoke_Throws_WhenModelNull()
        {
            var component = new GenericTableViewComponent();

            Assert.Throws<ArgumentNullException>(() => component.Invoke(null!));
        }

        [Fact]
        public void Invoke_Throws_WhenModelWrongType()
        {
            var component = new GenericTableViewComponent();

            Assert.Throws<ArgumentException>(() => component.Invoke(new object()));
        }

        [Fact]
        public void Invoke_ReturnsView_WhenModelValid()
        {
            var component = new GenericTableViewComponent();
            var model = new TableViewModel<string>();

            var result = component.Invoke(model);

            Assert.IsType<ViewViewComponentResult>(result);
        }
    }
}
