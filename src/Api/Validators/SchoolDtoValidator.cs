using Api.Contracts;
using FluentValidation;

namespace Api.Validators;

public class SchoolDtoValidator : AbstractValidator<SchoolDto>
{
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
