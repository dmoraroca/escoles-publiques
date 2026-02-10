using Web.Models;
using Web.Validators;
using Xunit;

namespace UnitTest.Validators
{
    public class StudentViewModelValidatorTests
    {
        [Fact]
        public void Validate_Fails_WhenRequiredFieldsMissing()
        {
            var validator = new StudentViewModelValidator();
            var model = new StudentViewModel();

            var result = validator.Validate(model);

            Assert.False(result.IsValid);
        }

        [Fact]
        public void Validate_Fails_WhenEmailInvalid()
        {
            var validator = new StudentViewModelValidator();
            var model = new StudentViewModel
            {
                FirstName = "A",
                LastName = "B",
                Email = "bad-email",
                SchoolId = 1
            };

            var result = validator.Validate(model);

            Assert.False(result.IsValid);
        }

        [Fact]
        public void Validate_Passes_WhenModelValid()
        {
            var validator = new StudentViewModelValidator();
            var model = new StudentViewModel
            {
                FirstName = "A",
                LastName = "B",
                Email = "a@b.com",
                SchoolId = 1
            };

            var result = validator.Validate(model);

            Assert.True(result.IsValid);
        }
    }
}
