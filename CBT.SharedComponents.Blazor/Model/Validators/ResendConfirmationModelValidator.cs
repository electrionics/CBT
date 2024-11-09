using FluentValidation;

using CBT.SharedComponents.Blazor.Model.Identity;

namespace CBT.SharedComponents.Blazor.Model.Validators
{
    public class ResendConfirmationModelValidator:AbstractValidator<ResendConfirmationModel>
    {
        public ResendConfirmationModelValidator()
        {
            RuleFor(m => m.Email)
                .NotEmpty()
                    .WithMessage("Электронная почта не может быть пустой.")
                .EmailAddress()
                    .WithMessage("Некорректный формат электронной почты.")
                .MaximumLength(100)
                    .WithMessage("Длина электронной почты не может превышать 100 символов.");
        }
    }
}
