using FluentValidation;
using Web.Models;

namespace Web.Validators;

public class AnnualFeeViewModelValidator : AbstractValidator<AnnualFeeViewModel>
{
    public AnnualFeeViewModelValidator()
    {
        RuleFor(x => x.EnrollmentId)
            .GreaterThan(0)
            .WithMessage("Has de seleccionar una matrícula vàlida");

        RuleFor(x => x.Amount)
            .NotEmpty()
            .WithMessage("L'import és obligatori")
            .GreaterThan(0)
            .WithMessage("L'import ha de ser superior a zero")
            .LessThanOrEqualTo(100000)
            .WithMessage("L'import no pot superar els 100.000€");

        RuleFor(x => x.DueDate)
            .NotEmpty()
            .WithMessage("La data de venciment és obligatòria")
            .GreaterThanOrEqualTo(DateTime.Now.AddDays(-365))
            .WithMessage("La data de venciment no pot ser anterior a fa un any");
    }
}
