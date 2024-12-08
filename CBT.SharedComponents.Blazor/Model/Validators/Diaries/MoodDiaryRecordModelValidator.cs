using FluentValidation;

namespace CBT.SharedComponents.Blazor.Model.Validators.Diaries
{
    public class MoodDiaryRecordModelValidator:AbstractValidator<MoodDiaryRecordModel>
    {
        public MoodDiaryRecordModelValidator() 
        {
            RuleFor(x => x.Date)
                .NotEmpty()
                    .WithMessage("Выберите дату записи.");
            RuleFor(x => x.Time)
                .NotEmpty()
                    .WithMessage("Выберите час записи.");
            RuleFor(x => x.Events)
                .MaximumLength(1000)
                    .WithMessage("Длина не может превышать 1000 символов.");
            RuleFor(x => x.Value)
                .NotEmpty()
                    .WithMessage("Значение обязательно для заполнения.")
                .InclusiveBetween(0, 100)
                    .WithMessage("Оцените настроение по шкале от 0 до 100");
        }
    }
}
