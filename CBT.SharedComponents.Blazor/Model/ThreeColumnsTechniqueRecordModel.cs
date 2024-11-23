using CBT.Domain.Entities;
using CBT.Domain.Entities.Enums;

namespace CBT.SharedComponents.Blazor.Model
{
    public class ThreeColumnsTechniqueRecordModel : IThoughtRecordModel<ThreeColumnsTechniqueRecordModel> // техника трех колонок с. 94
    {
        public int Id { get;set; }

        public string Thought { get; set; }
        public List<int> Errors { get; set; }
        public string? RationalAnswer { get; set; }

        public bool Sent { get; set; }


        public ThreeColumnsTechniqueRecordModel() 
        {
            Errors = [];
        }


        #region Convert

        public ThreeColumnsTechniqueRecordModel? Convert(AutomaticThought data)
        {
            if (data == null)
                return null;

            return new ThreeColumnsTechniqueRecordModel()
            {
                Id = data.Id,
                Thought = data.Thought,
                RationalAnswer = data.RationalAnswer,
                Errors = data.CognitiveErrors.Where(x => !x.IsReview).Select(y => y.CognitiveErrorId).ToList(),
                Sent = data.Sent,
            };
        }

        #endregion


        #region ConvertBack

        public AutomaticThought ConvertBack(int patientId, DiaryType type, AutomaticThought? data = null)
        {
            var model = this;

            data ??= new AutomaticThought();

            data.Id = model.Id;
            data.Thought = model.Thought;
            data.RationalAnswer = model.RationalAnswer;
            data.CognitiveErrors = model.Errors?
                .Select(x => new ThoughtCognitiveError
                {
                    ThoughtId = data.Id,
                    CognitiveErrorId = x
                }).ToList() ?? [];
            data.Emotions = [];
            data.Type = type;
            data.PatientId = patientId;
            data.Sent = model.Sent;

            return data;
        }

        #endregion
    }
}
