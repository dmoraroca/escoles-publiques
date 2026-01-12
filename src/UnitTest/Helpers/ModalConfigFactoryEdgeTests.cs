using Xunit;
using Web.Helpers;
using Web.Models;
using System.Linq;
using System.Collections.Generic;

namespace UnitTest.Helpers
{
    public class ModalConfigFactoryEdgeTests
    {
        [Fact]
        public void GetStudentModalConfig_IncludesEmailAndBirthDate()
        {
            var config = ModalConfigFactory.GetStudentModalConfig(new List<SelectOption>());
            var fields = config.Fields.Select(f => f.Name).ToList();

            Assert.Contains("Email", fields);
            Assert.Contains("BirthDate", fields);
        }

        [Fact]
        public void GetSchoolModalConfig_HasCodeField()
        {
            var config = ModalConfigFactory.GetSchoolModalConfig(new List<SelectOption>());
            var code = config.Fields.FirstOrDefault(f => f.Name == "Code");
            Assert.NotNull(code);
        }
    }
}
