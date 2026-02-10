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
            Assert.Contains(config.Fields, f => f.Name == "ScopeId");
            Assert.Contains(config.Fields, f => f.Name == "IsFavorite");
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

        [Fact]
        public void GetEnrollmentModalConfig_ReturnsConfig_WithExpectedFields()
        {
            var studentOptions = new System.Collections.Generic.List<Web.Models.SelectOption> { new Web.Models.SelectOption { Value = "1", Text = "Alumne 1" } };
            var schoolOptions = new System.Collections.Generic.List<Web.Models.SelectOption> { new Web.Models.SelectOption { Value = "2", Text = "Escola 2" } };
            var config = Web.Helpers.ModalConfigFactory.GetEnrollmentModalConfig(studentOptions, schoolOptions);
            Assert.NotNull(config);
            Assert.Equal("Inscripció", config.EntityName);
            Assert.Contains(config.Fields, f => f.Name == "StudentId");
            Assert.Contains(config.Fields, f => f.Name == "SchoolId");
            Assert.Contains(config.Fields, f => f.Name == "AcademicYear");
            Assert.Contains(config.Fields, f => f.Name == "CourseName");
            Assert.Contains(config.Fields, f => f.Name == "Status");
        }

        [Fact]
        public void GetAnnualFeeModalConfig_ReturnsConfig_WithExpectedFields()
        {
            var enrollmentOptions = new System.Collections.Generic.List<Web.Models.SelectOption> { new Web.Models.SelectOption { Value = "3", Text = "Inscripció 3" } };
            var config = Web.Helpers.ModalConfigFactory.GetAnnualFeeModalConfig(enrollmentOptions);
            Assert.NotNull(config);
            Assert.Equal("Quota", config.EntityName);
            Assert.Contains(config.Fields, f => f.Name == "EnrollmentId");
            Assert.Contains(config.Fields, f => f.Name == "Amount");
            Assert.Contains(config.Fields, f => f.Name == "Currency");
            Assert.Contains(config.Fields, f => f.Name == "DueDate");
            Assert.Contains(config.Fields, f => f.Name == "IsPaid");
            Assert.Contains(config.Fields, f => f.Name == "PaymentRef");
        }
    }
}
