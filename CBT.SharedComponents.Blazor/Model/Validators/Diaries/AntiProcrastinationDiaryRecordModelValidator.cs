using FluentValidation;

namespace CBT.SharedComponents.Blazor.Model.Validators.Diaries
{
    public class AntiProcrastinationDiaryRecordModelValidator : AbstractValidator<AntiProcrastinationDiaryRecordModel>
    {
        public AntiProcrastinationDiaryRecordModelValidator()
        {
            RuleFor(x => x.PlanDate)
                .NotEmpty()
                    .WithMessage("Заполните планируемую дату.");
            RuleFor(x => x.Task)
                .NotEmpty()
                    .WithMessage("Заполните текст задания.");
            RuleFor(x => x.SupposedEffort)
                .InclusiveBetween(0, 100)
                    .WithMessage("Оцените предполагаемые усилия по шкале от 0 до 100.");
            RuleFor(x => x.SupposedPleasure)
                .InclusiveBetween(0, 100)
                    .WithMessage("Оцените предполагаемое удовольствие по шкале от 0 до 100.");
            RuleFor(x => x.ActualEffort)
                .InclusiveBetween(0, 100)
                    .WithMessage("Оцените реальные усилия по шкале от 0 до 100.");
            RuleFor(x => x.ActualPleasure)
                .InclusiveBetween(0, 100)
                    .WithMessage("Оцените реальное удовольствие по шкале от 0 до 100.");
        }
    }
}
