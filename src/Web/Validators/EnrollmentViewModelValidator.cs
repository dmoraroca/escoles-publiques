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

        RuleFor(x => x.Year)
            .NotEmpty()
            .WithMessage("L'any és obligatori")
            .InclusiveBetween(2000, 2100)
            .WithMessage("L'any ha d'estar entre 2000 i 2100");

        RuleFor(x => x.EnrollmentType)
            .MaximumLength(50)
            .WithMessage("El tipus de matrícula no pot tenir més de 50 caràcters")
            .When(x => !string.IsNullOrEmpty(x.EnrollmentType));
    }
}
