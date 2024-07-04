using FluentValidation;

using CBT.Web.Blazor.Data.Model.Identity;

namespace CBT.Web.Blazor.Data.Model.Validators
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
