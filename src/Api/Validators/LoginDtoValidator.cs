using Api.Contracts;
using FluentValidation;

namespace Api.Validators;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("L'email és obligatori.")
            .EmailAddress().WithMessage("L'email no és vàlid.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contrasenya és obligatòria.")
            .MinimumLength(6).WithMessage("La contrasenya ha de tenir almenys 6 caràcters.");
    }
}
