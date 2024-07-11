using FluentValidation;

using CBT.SharedComponents.Blazor.Model.Identity;

namespace CBT.SharedComponents.Blazor.Model.Validators
{
    public class RegisterModelValidator : AbstractValidator<RegisterModel>
    {
        public RegisterModelValidator() 
        {
            RuleFor(m => m.RoleTypes)
                .NotEmpty()
                    .WithMessage("Выберите тип пользователя.");

            RuleFor(m => m.Name)
                .NotEmpty()
                    .WithMessage("Имя обязательно для заполнения.")
                .MaximumLength(50)
                    .WithMessage("Длина имени не должна превышать 50 символов.");

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
        }
    }
}
