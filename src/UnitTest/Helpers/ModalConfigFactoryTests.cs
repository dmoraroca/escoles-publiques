using Xunit;

namespace UnitTest.Helpers
{
    public class ModalConfigFactoryTests
    {
        [Fact]
        public void GetSchoolModalConfig_ReturnsConfig_WithExpectedFields()
        {
            var scopeOptions = new System.Collections.Generic.List<Web.Models.SelectOption> { new Web.Models.SelectOption { Value = "1", Text = "Àmbit 1" } };
            var config = Web.Helpers.ModalConfigFactory.GetSchoolModalConfig(scopeOptions);
            Assert.NotNull(config);
            Assert.Equal("Escola", config.EntityName);
            Assert.Contains(config.Fields, f => f.Name == "Code");
            Assert.Contains(config.Fields, f => f.Name == "Name");
            Assert.Contains(config.Fields, f => f.Name == "City");
            Assert.Contains(config.Fields, f => f.Name == "Scope");
        }

        [Fact]
        public void GetStudentModalConfig_ReturnsConfig_WithExpectedFields()
        {
            var schoolOptions = new System.Collections.Generic.List<Web.Models.SelectOption> { new Web.Models.SelectOption { Value = "1", Text = "Escola 1" } };
            var config = Web.Helpers.ModalConfigFactory.GetStudentModalConfig(schoolOptions);
            Assert.NotNull(config);
            Assert.Equal("Alumne", config.EntityName);
            Assert.Contains(config.Fields, f => f.Name == "FirstName");
            Assert.Contains(config.Fields, f => f.Name == "LastName");
            Assert.Contains(config.Fields, f => f.Name == "Email");
            Assert.Contains(config.Fields, f => f.Name == "BirthDate");
            Assert.Contains(config.Fields, f => f.Name == "SchoolId");
        }
    }
}
