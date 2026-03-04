using Api.Contracts;
using FluentValidation;

namespace Api.Validators;

public class EnrollmentDtoInValidator : AbstractValidator<EnrollmentDtoIn>
{
    public EnrollmentDtoInValidator()
    {
        RuleFor(x => x.StudentId)
            .GreaterThan(0).WithMessage("StudentId ha de ser major que 0.");

        RuleFor(x => x.AcademicYear)
            .NotEmpty().WithMessage("L'any acadèmic és obligatori.")
            .MaximumLength(20);

        RuleFor(x => x.CourseName)
            .MaximumLength(120)
            .When(x => !string.IsNullOrWhiteSpace(x.CourseName));

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("L'estat és obligatori.")
            .MaximumLength(30);

        RuleFor(x => x.SchoolId)
            .GreaterThan(0).WithMessage("SchoolId ha de ser major que 0.");

        RuleFor(x => x.EnrolledAt)
            .Must(d => !d.HasValue || d.Value <= DateTime.UtcNow)
            .When(x => x.EnrolledAt.HasValue)
            .WithMessage("La data d'inscripció no pot ser futura.");
    }
}
