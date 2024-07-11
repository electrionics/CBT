using CBT.SharedComponents.Blazor.Model.Identity;
using FluentValidation;

namespace CBT.SharedComponents.Blazor.Model.Validators
{
    public class ResetPasswordModelValidator : AbstractValidator<ResetPasswordModel>
    {
        public ResetPasswordModelValidator() 
        {
            RuleFor(m => m.Email)
                .NotEmpty()
                    .WithMessage("Электронная почта не может быть пустой.")
                .EmailAddress()
                    .WithMessage("Некорректный формат электронной почты.")
                .MaximumLength(100)
                    .WithMessage("Длина электронной почты не может превышать 100 символов.");

            RuleFor(m => m.Password)
                .NotEmpty()
                    .WithMessage("Пароль не может быть пустым.")
                .Length(8, 100)
                    .WithMessage("Пароль должен иметь длину не меньше 8 символов.");

            RuleFor(m => m.ConfirmPassword)
                .NotEmpty()
                    .WithMessage("Подтвердите пароль.")
                .Equal(x => x.Password)
                    .WithMessage("Пароли не совпадают");

            RuleFor(m => m.Code)
                .NotEmpty()
                    .WithMessage("Код не может быть пустым")
                .Length(0, 1000)
                    .WithMessage("Код должен иметь длину не больше 1000 символов.");
        }
    }
}
