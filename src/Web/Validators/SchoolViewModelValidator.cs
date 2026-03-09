using FluentValidation;
using Web.Models;

namespace Web.Validators;
/// <summary>
/// Validates incoming data for school view model.
/// </summary>
public class SchoolViewModelValidator : AbstractValidator<SchoolViewModel>
{
            /// <summary>
            /// Initializes a new instance of the SchoolViewModelValidator class with its required dependencies.
            /// </summary>
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
