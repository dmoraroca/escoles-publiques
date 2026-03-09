using FluentValidation;
using Web.Models;

namespace Web.Validators;
/// <summary>
/// Validates incoming data for student view model.
/// </summary>
public class StudentViewModelValidator : AbstractValidator<StudentViewModel>
{
            /// <summary>
            /// Initializes a new instance of the StudentViewModelValidator class with its required dependencies.
            /// </summary>
            public StudentViewModelValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("El nom és obligatori")
            .MaximumLength(100)
            .WithMessage("El nom no pot tenir més de 100 caràcters");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Els cognoms són obligatoris")
            .MaximumLength(100)
            .WithMessage("Els cognoms no poden tenir més de 100 caràcters");

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("Email invàlid")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.SchoolId)
            .GreaterThan(0)
            .WithMessage("Has de seleccionar una escola vàlida");
    }
}
