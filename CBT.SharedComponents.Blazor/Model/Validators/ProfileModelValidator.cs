using FluentValidation;

namespace CBT.SharedComponents.Blazor.Model.Validators
{
    public class ProfileModelValidator:AbstractValidator<ProfileModel>
    {
        public ProfileModelValidator() 
        {
            RuleFor(m => m.DisplayName)
                .NotEmpty()
                    .WithMessage("Имя обязательно для заполнения.")
                .MaximumLength(50)
                    .WithMessage("Длина имени не должна превышать 50 символов.");
        }
    }
}
