using FluentValidation;
using Web.Models;

namespace Web.Validators;

public class AnnualFeeViewModelValidator : AbstractValidator<AnnualFeeViewModel>
{
    public AnnualFeeViewModelValidator()
    {
        RuleFor(x => x.EnrollmentId)
            .GreaterThan(0)
            .WithMessage("Has de seleccionar una inscripció vàlida");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("L'import ha de ser superior a zero");

        RuleFor(x => x.DueDate)
            .NotEmpty()
            .WithMessage("La data de venciment és obligatòria");
    }
}
