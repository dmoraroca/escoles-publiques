using Web.Models;
using Web.Validators;
using Xunit;

namespace UnitTest.Validators
{
    public class EnrollmentViewModelValidatorTests
    {
        [Fact]
        public void Validate_Fails_WhenRequiredFieldsMissing()
        {
            var validator = new EnrollmentViewModelValidator();
            var model = new EnrollmentViewModel();

            var result = validator.Validate(model);

            Assert.False(result.IsValid);
        }

        [Fact]
        public void Validate_Passes_WhenModelValid()
        {
            var validator = new EnrollmentViewModelValidator();
            var model = new EnrollmentViewModel
            {
                StudentId = 1,
                AcademicYear = "2025",
                Status = "Active"
            };

            var result = validator.Validate(model);

            Assert.True(result.IsValid);
        }
    }
}
