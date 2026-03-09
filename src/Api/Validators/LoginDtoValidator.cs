using Api.Contracts;
using FluentValidation;

namespace Api.Validators;
/// <summary>
/// Validates incoming data for login dto.
/// </summary>
public class LoginDtoValidator : AbstractValidator<LoginDto>
{
            /// <summary>
            /// Initializes a new instance of the LoginDtoValidator class with its required dependencies.
            /// </summary>
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
