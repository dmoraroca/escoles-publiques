using FluentValidation;
using Web.Models;

namespace Web.Validators;

/// <summary>
/// Validador FluentValidation per la vista d'inscripció. Comprova alumne, any acadèmic i estat.
/// </summary>
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
