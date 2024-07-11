using FluentValidation;

using CBT.SharedComponents.Blazor.Model.Identity;

namespace CBT.SharedComponents.Blazor.Model.Validators
{
    public class LoginModelValidator:AbstractValidator<LoginModel>
    {
        public LoginModelValidator() 
        {
            RuleFor(m => m.Email)
                .NotEmpty()
                    .WithMessage("Электронная почта не может быть пустой.")
                .EmailAddress()
                    .WithMessage("Некорректный формат электронной почты.");

            RuleFor(m => m.Password)
                .NotEmpty()
                    .WithMessage("Пароль не может быть пустым.");
        }
    }
}
