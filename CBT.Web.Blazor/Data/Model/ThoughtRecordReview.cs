using CBT.Web.Blazor.Data.Entities;
using CBT.Web.Blazor.Data.Model.Enums;

namespace CBT.Web.Blazor.Data.Model
{
    public class ThoughtRecordReview<T>
        where T : IThoughtRecordModel<T>, new()
    {
        public T Value { get; set; }

        public ReviewRecordState State { get; set; }

        public string RationalAnswerComment { get; set; }
        
        public List<int> ReviewedErrors { get; set; }

        public bool SentBack { get; set; }

        public string PatientDisplayName { get; set; }


        public static ThoughtRecordReview<T>? Convert(AutomaticThought data)
        {
            if (data == null)
                return null;

#pragma warning disable CS8601 // Possible null reference assignment.
            return new ThoughtRecordReview<T>
            {
                Value = new T().Convert(data),
                State = data.PsychologistReviews.Any()
                    ? ReviewRecordState.Reviewed
                    : ReviewRecordState.Pending,
                SentBack = data.SentBack,
                RationalAnswerComment = data.PsychologistReviews.FirstOrDefault()?.RationalAnswerComment ?? string.Empty,
                ReviewedErrors = data.CognitiveErrors.Where(x => x.IsReview).Select(x => x.CognitiveErrorId).ToList(),
            };
#pragma warning restore CS8601 // Possible null reference assignment.
        }

        public static AutomaticThought ConvertBack(ThoughtRecordReview<T> model, int psychologistId, AutomaticThought data)
        {
            data.PsychologistReviews.RemoveAll(x => true);
            data.PsychologistReviews.Add(new ThoughtPsychologistReview
            {
                PsychologistId = psychologistId,
                RationalAnswerComment = model.RationalAnswerComment,
                ThoughtId = model.Value.Id,
            });
            data.CognitiveErrors.RemoveAll(x => x.IsReview);
            data.CognitiveErrors.AddRange(model.ReviewedErrors?.Select(x => new ThoughtCognitiveError
            {
                CognitiveErrorId = x,
                PsychologistId = psychologistId,
                IsReview = true,
                ThoughtId = model.Value.Id
            }) ?? Enumerable.Empty<ThoughtCognitiveError>());

            return data;
        }
    }
}
