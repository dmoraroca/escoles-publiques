using System.Collections.Generic;
using Web.Models;
using Xunit;

namespace UnitTest.Models
{
    public class TableViewModelTests
    {
        [Fact]
        public void Defaults_AreExpected()
        {
            var model = new TableViewModel<string>();

            Assert.True(model.HasSearch);
            Assert.False(model.HasPagination);
            Assert.Equal(10, model.PageSize);
            Assert.Equal(1, model.CurrentPage);
            Assert.Equal("Id", model.IdPropertyName);
        }

        [Fact]
        public void CanSetProperties()
        {
            var model = new TableViewModel<string>
            {
                Data = new List<string> { "A" },
                EntityName = "Test",
                ControllerName = "TestController",
                CustomCssClass = "custom"
            };

            Assert.Single(model.Data);
            Assert.Equal("Test", model.EntityName);
            Assert.Equal("TestController", model.ControllerName);
            Assert.Equal("custom", model.CustomCssClass);
        }

        [Fact]
        public void ColumnConfig_And_TableAction_AreConfigurable()
        {
            var column = new ColumnConfig
            {
                PropertyName = "Name",
                DisplayName = "Nom",
                IsSortable = false,
                IsVisibleOnMobile = false,
                Format = "C2",
                CustomRender = _ => "X",
                IsActionColumn = true
            };

            var action = new TableAction
            {
                ActionName = "Edit",
                DisplayText = "Editar",
                CssClass = "btn-primary",
                Icon = "edit",
                Controller = "Schools",
                RequiresConfirmation = true,
                ConfirmationMessage = "Confirm?"
            };

            Assert.Equal("X", column.CustomRender?.Invoke(new object()));
            Assert.True(action.RequiresConfirmation);
        }
    }
}
