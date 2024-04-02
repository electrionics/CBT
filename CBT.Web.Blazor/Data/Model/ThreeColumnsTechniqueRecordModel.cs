using CBT.Web.Blazor.Data.Entities;
using CBT.Web.Blazor.Data.Entities.Enums;

namespace CBT.Web.Blazor.Data.Model
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
            Errors = new List<int>();
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

            if (data == null)
               data = new AutomaticThought();

            data.Id = model.Id;
            data.Thought = model.Thought;
            data.RationalAnswer = model.RationalAnswer;
            data.CognitiveErrors = model.Errors?
                .Select(x => new ThoughtCognitiveError
                {
                    ThoughtId = data.Id,
                    CognitiveErrorId = x
                }).ToList() ?? new List<ThoughtCognitiveError>();
            data.Emotions = new List<ThoughtEmotion>();
            data.Type = type;
            data.PatientId = patientId;
            data.Sent = model.Sent;

            return data;
        }

        #endregion
    }
}
