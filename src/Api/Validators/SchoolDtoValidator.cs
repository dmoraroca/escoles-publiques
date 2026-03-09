using Api.Contracts;
using FluentValidation;

namespace Api.Validators;
/// <summary>
/// Validates incoming data for school dto.
/// </summary>
public class SchoolDtoValidator : AbstractValidator<SchoolDto>
{
            /// <summary>
            /// Initializes a new instance of the SchoolDtoValidator class with its required dependencies.
            /// </summary>
            public SchoolDtoValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("El codi és obligatori.")
            .MaximumLength(20);

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nom és obligatori.")
            .MaximumLength(200);

        RuleFor(x => x.City)
            .MaximumLength(120)
            .When(x => !string.IsNullOrWhiteSpace(x.City));

        RuleFor(x => x.ScopeId)
            .GreaterThan(0)
            .When(x => x.ScopeId.HasValue)
            .WithMessage("ScopeId ha de ser major que 0 quan s'informa.");
    }
}
