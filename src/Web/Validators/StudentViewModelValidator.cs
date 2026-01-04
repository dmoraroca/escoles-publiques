using FluentValidation;
using Web.Models;

namespace Web.Validators;

public class StudentViewModelValidator : AbstractValidator<StudentViewModel>
{
    public StudentViewModelValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("El nom de l'alumne és obligatori")
            .MaximumLength(100)
            .WithMessage("El nom no pot tenir més de 100 caràcters");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Els cognoms de l'alumne són obligatoris")
            .MaximumLength(100)
            .WithMessage("Els cognoms no poden tenir més de 100 caràcters");

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("L'adreça d'email no és vàlida")
            .MaximumLength(150)
            .WithMessage("L'email no pot tenir més de 150 caràcters")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.BirthDate)
            .LessThan(DateTime.Now)
            .WithMessage("La data de naixement ha de ser anterior a la data actual")
            .When(x => x.BirthDate.HasValue);

        RuleFor(x => x.SchoolId)
            .GreaterThan(0)
            .WithMessage("Has de seleccionar una escola vàlida")
            .When(x => x.SchoolId > 0);
    }
}
