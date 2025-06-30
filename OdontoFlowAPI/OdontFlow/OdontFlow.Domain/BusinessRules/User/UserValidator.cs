using FluentValidation;
using OdontFlow.Domain.DTOs;  

namespace OdontFlow.Domain.BusinessRules.User
{
    public class RegisterUserValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterUserValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email es requerido").EmailAddress().WithMessage("Email no es valido"); 
        }
    }

    public class ChangeFirstTimePasswordValidator : AbstractValidator<ResetFirstPasswordRequest>
    {
        public ChangeFirstTimePasswordValidator()
        {
            RuleFor(x => x.NewPassword).NotEmpty().WithMessage("En nuevo password es requerido");
        }
    }
}
