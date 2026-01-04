using FluentValidation;
using Web.Models;

namespace Web.Validators;

public class SchoolViewModelValidator : AbstractValidator<SchoolViewModel>
{
    public SchoolViewModelValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("El codi de l'escola és obligatori")
            .MaximumLength(10)
            .WithMessage("El codi no pot tenir més de 10 caràcters");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("El nom de l'escola és obligatori")
            .MaximumLength(200)
            .WithMessage("El nom no pot tenir més de 200 caràcters");

        RuleFor(x => x.City)
            .NotEmpty()
            .WithMessage("La ciutat és obligatòria")
            .MaximumLength(100)
            .WithMessage("La ciutat no pot tenir més de 100 caràcters");

        RuleFor(x => x.Scope)
            .MaximumLength(50)
            .WithMessage("L'àmbit no pot tenir més de 50 caràcters")
            .When(x => !string.IsNullOrEmpty(x.Scope));
    }
}
