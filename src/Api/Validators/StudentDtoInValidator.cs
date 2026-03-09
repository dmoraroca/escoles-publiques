using Api.Contracts;
using FluentValidation;

namespace Api.Validators;
/// <summary>
/// Validates incoming data for student dto in.
/// </summary>
public class StudentDtoInValidator : AbstractValidator<StudentDtoIn>
{
            /// <summary>
            /// Initializes a new instance of the StudentDtoInValidator class with its required dependencies.
            /// </summary>
            public StudentDtoInValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("El nom és obligatori.")
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Els cognoms són obligatoris.")
            .MaximumLength(150);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("L'email és obligatori.")
            .EmailAddress().WithMessage("L'email no és vàlid.")
            .MaximumLength(200);

        RuleFor(x => x.SchoolId)
            .GreaterThan(0).WithMessage("SchoolId ha de ser major que 0.");

        RuleFor(x => x.BirthDate)
            .Must(d => !d.HasValue || d.Value <= DateOnly.FromDateTime(DateTime.UtcNow))
            .When(x => x.BirthDate.HasValue)
            .WithMessage("La data de naixement no pot ser futura.");
    }
}
