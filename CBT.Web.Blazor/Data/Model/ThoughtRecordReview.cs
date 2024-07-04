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


        public static ThoughtRecordReview<T>? Convert(AutomaticThought data, int? psychologistId = null)
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
                SentBack = data.PsychologistReviews
                    .Any(x => x.SentBack && x.PsychologistId == psychologistId),
                RationalAnswerComment = data.PsychologistReviews
                    .FirstOrDefault(x => x.PsychologistId == psychologistId)?.RationalAnswerComment ?? string.Empty,
                ReviewedErrors = data.CognitiveErrors
                    .Where(x => x.PsychologistId == psychologistId)
                    .Select(x => x.CognitiveErrorId)
                    .ToList(),
                PatientDisplayName = data.Patient.DisplayName
            };
#pragma warning restore CS8601 // Possible null reference assignment.
        }

        public static AutomaticThought ConvertBack(ThoughtRecordReview<T> model, int psychologistId, AutomaticThought data)
        {
            data.PsychologistReviews.RemoveAll(x => 
                x.PsychologistId == psychologistId);
            data.PsychologistReviews.Add(new ThoughtPsychologistReview
            {
                PsychologistId = psychologistId,
                ThoughtId = model.Value.Id,
                RationalAnswerComment = model.RationalAnswerComment,
            });

            data.CognitiveErrors.RemoveAll(x => 
                x.PsychologistId == psychologistId);
            data.CognitiveErrors.AddRange(model.ReviewedErrors?.Select(x => new ThoughtCognitiveError
            {
                CognitiveErrorId = x,
                PsychologistId = psychologistId,
                ReviewerId = psychologistId,
                ThoughtId = model.Value.Id
            }) ?? Enumerable.Empty<ThoughtCognitiveError>());

            return data;
        }
    }
}
