using System.Threading.Tasks;
using Xunit;
using Web.Helpers;
using UnitTest.Fakes;

namespace UnitTest.Integration
{
    public class ModalConfigFactoryIntegrationTests
    {
        [Fact]
        public async Task GetStudentModalConfig_Integration_ReturnsConfigWithFakeSchools()
        {
            // Arrange
            var fakeSchools = FakeData.GetFakeSchoolOptions();
            // Act
            var config = ModalConfigFactory.GetStudentModalConfig(fakeSchools);
            // Assert
            Assert.NotNull(config);
            Assert.Equal("Alumne", config.EntityName);
            Assert.Contains(config.Fields, f => f.Name == "SchoolId");
            var schoolField = config.Fields.Find(f => f.Name == "SchoolId");
            Assert.NotNull(schoolField);
            Assert.NotNull(schoolField.Options);
            Assert.Equal(2, schoolField.Options.Count);
            Assert.Contains(schoolField.Options, o => o.Text == "Escola Test 1");
            Assert.Contains(schoolField.Options, o => o.Text == "Escola Test 2");
        }
    }
}
