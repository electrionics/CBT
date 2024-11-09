using FluentValidation;

namespace CBT.SharedComponents.Blazor.Model.Validators.Diaries
{
    public class ThreeColumnsTechniqueRecordModelValidator : AbstractValidator<ThreeColumnsTechniqueRecordModel>
    {
        public ThreeColumnsTechniqueRecordModelValidator()
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
        }
    }
}
