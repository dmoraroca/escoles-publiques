using FluentValidation;
using Web.Models;

namespace Web.Validators;
/// <summary>
/// Validates incoming data for enrollment view model.
/// </summary>
public class EnrollmentViewModelValidator : AbstractValidator<EnrollmentViewModel>
{
            /// <summary>
            /// Initializes a new instance of the EnrollmentViewModelValidator class with its required dependencies.
            /// </summary>
            public EnrollmentViewModelValidator()
    {
        RuleFor(x => x.StudentId)
            .GreaterThan(0)
            .WithMessage("Has de seleccionar un alumne vàlid");

        RuleFor(x => x.AcademicYear)
            .NotEmpty()
            .WithMessage("L'any acadèmic és obligatori");

        RuleFor(x => x.Status)
            .NotEmpty()
            .WithMessage("L'estat és obligatori");
    }
}
