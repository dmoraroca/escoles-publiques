using FluentValidation;
using Web.Models;

namespace Web.Validators;

public class EnrollmentViewModelValidator : AbstractValidator<EnrollmentViewModel>
{
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
