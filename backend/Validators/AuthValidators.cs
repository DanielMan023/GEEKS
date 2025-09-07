using FluentValidation;
using GEEKS.Dto;
using GEEKS.Utils;

namespace GEEKS.Validators
{
    /// <summary>
    /// Validador para LoginDTO
    /// </summary>
    public class LoginValidator : AbstractValidator<LoginDTO>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(Constants.ErrorMessages.Required)
                .EmailAddress().WithMessage(Constants.ErrorMessages.InvalidEmail)
                .MaximumLength(Constants.Validation.EmailMaxLength)
                .WithMessage($"El email no puede exceder {Constants.Validation.EmailMaxLength} caracteres");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(Constants.ErrorMessages.Required)
                .MinimumLength(Constants.Validation.PasswordMinLength)
                .WithMessage(Constants.ErrorMessages.InvalidPassword);
        }
    }

    /// <summary>
    /// Validador para RegisterDTO
    /// </summary>
    public class RegisterValidator : AbstractValidator<RegisterDTO>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(Constants.ErrorMessages.Required)
                .EmailAddress().WithMessage(Constants.ErrorMessages.InvalidEmail)
                .MaximumLength(Constants.Validation.EmailMaxLength)
                .WithMessage($"El email no puede exceder {Constants.Validation.EmailMaxLength} caracteres");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(Constants.ErrorMessages.Required)
                .MinimumLength(Constants.Validation.PasswordMinLength)
                .WithMessage(Constants.ErrorMessages.InvalidPassword)
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)")
                .WithMessage("La contraseña debe contener al menos una letra minúscula, una mayúscula y un número");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage(Constants.ErrorMessages.Required)
                .MaximumLength(Constants.Validation.FirstNameMaxLength)
                .WithMessage($"El nombre no puede exceder {Constants.Validation.FirstNameMaxLength} caracteres")
                .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$")
                .WithMessage("El nombre solo puede contener letras y espacios");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage(Constants.ErrorMessages.Required)
                .MaximumLength(Constants.Validation.LastNameMaxLength)
                .WithMessage($"El apellido no puede exceder {Constants.Validation.LastNameMaxLength} caracteres")
                .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$")
                .WithMessage("El apellido solo puede contener letras y espacios");
        }
    }
}
