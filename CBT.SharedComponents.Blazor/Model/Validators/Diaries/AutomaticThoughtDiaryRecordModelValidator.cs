using FluentValidation;

namespace CBT.SharedComponents.Blazor.Model.Validators.Diaries
{
    internal class AutomaticThoughtDiaryRecordModelValidator:AbstractValidator<AutomaticThoughtDiaryRecordModel>
    {
        public AutomaticThoughtDiaryRecordModelValidator() : base()
        {
            RuleFor(x => x.Thought)
                .NotEmpty() // user
                    .WithMessage("Мысль обязательна для заполнения.")
                .Length(0, 1000) // user
                    .WithMessage("Сократите мысль до 1000 символов или разбейте на несколько записей. Текущая длина: {TotalLength} символов.");
            RuleFor(x => x.Errors)
                .NotEmpty() // user
                    .WithMessage("Выберите как минимум 1 когнитивное искажение, чтобы выработать привычку не откладывать анализ мыслей на потом.");
            RuleFor(x => x.RationalAnswer)
                .Length(0, 2000) // user
                    .WithMessage("Сократите рациональный ответ до 2000 символов или разбейте на несколько записей. Текущая длина: {TotalLength} символов.");
            RuleFor(x => x.Situation)
                .Length(0, 300) // user
                    .WithMessage("Сократите описание ситуации, вызвавшей автоматическую мысль, до 300 символов. Текущая длина: {TotalLength} символов.");
            RuleFor(x => x.BindEmotionIds)
                .Must(x => x.Count > 0) // user
                    .WithMessage("Выберите как минимум 1 эмоцию.");

            RuleFor(x => x.BeginningEmotionValues)
                .Must(x => x.All(y => y.Value >= 0 && y.Value <= 100)) // business
                    .WithMessage("Уровень переживания каждой эмоции должен быть в интервале от 0 до 100.");
            RuleFor(x => x.ResultingEmotionValues)
                .Must((x, p) => p.Count == x.BeginningEmotionValues.Count) // business
                    .WithMessage("Количество эмоций до и после проработки должно совпадать.") // если можно будет добавлять положительные эмоции, то количество может не совпадать
                .Must(x => x.All(y => y.Value >= 0 && y.Value <= 100)) // business
                    .WithMessage("Уровень переживания каждой эмоции должен быть в интервале от 0 до 100.");
        }
    }
}