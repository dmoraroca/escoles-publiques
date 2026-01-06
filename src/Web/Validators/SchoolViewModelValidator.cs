using FluentValidation;
using Web.Models;

namespace Web.Validators;

/// <summary>
/// Validador FluentValidation per la vista d'escola. Comprova codi, nom i ciutat.
/// </summary>
public class SchoolViewModelValidator : AbstractValidator<SchoolViewModel>
{
    public SchoolViewModelValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("El codi és obligatori")
            .MaximumLength(10)
            .WithMessage("El codi no pot tenir més de 10 caràcters");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("El nom és obligatori")
            .MaximumLength(200)
            .WithMessage("El nom no pot tenir més de 200 caràcters");

        RuleFor(x => x.City)
            .NotEmpty()
            .WithMessage("La ciutat és obligatòria")
            .MaximumLength(100)
            .WithMessage("La ciutat no pot tenir més de 100 caràcters");
    }
}
