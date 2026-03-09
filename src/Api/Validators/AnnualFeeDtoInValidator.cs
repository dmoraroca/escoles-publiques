using Api.Contracts;
using FluentValidation;

namespace Api.Validators;
/// <summary>
/// Validates incoming data for annual fee dto in.
/// </summary>
public class AnnualFeeDtoInValidator : AbstractValidator<AnnualFeeDtoIn>
{
            /// <summary>
            /// Initializes a new instance of the AnnualFeeDtoInValidator class with its required dependencies.
            /// </summary>
            public AnnualFeeDtoInValidator()
    {
        RuleFor(x => x.EnrollmentId)
            .GreaterThan(0).WithMessage("EnrollmentId ha de ser major que 0.");

        RuleFor(x => x.Amount)
            .GreaterThan(0m).WithMessage("L'import ha de ser major que 0.");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("La moneda és obligatòria.")
            .Length(3).WithMessage("La moneda ha de tenir 3 caràcters.")
            .Matches("^[A-Za-z]{3}$").WithMessage("La moneda ha de ser un codi ISO de 3 lletres.");

        RuleFor(x => x.PaymentRef)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.PaymentRef));
    }
}
