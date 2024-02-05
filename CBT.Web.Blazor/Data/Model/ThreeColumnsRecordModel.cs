using CBT.Web.Blazor.Data.Entities;

namespace CBT.Web.Blazor.Data.Model
{
    public class ThreeColumnsRecordModel : IThoughtRecordModel<ThreeColumnsRecordModel> // техника трех колонок с. 94
    {
        public int Id { get;set; }

        public string Thought { get; set; }
        public List<int> Errors { get; set; }
        public string? RationalAnswer { get; set; }

        public bool Sent { get; set; }


        public ThreeColumnsRecordModel() 
        {
            Errors = new List<int>();
        }


        #region Convert

        public ThreeColumnsRecordModel? Convert(AuthomaticThoughtDiaryRecord data)
        {
            if (data == null)
                return null;

            return new ThreeColumnsRecordModel()
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

        public AuthomaticThoughtDiaryRecord ConvertBack(int patientId, AuthomaticThoughtDiaryRecord? data = null)
        {
            var model = this;

            if (data == null)
               data = new AuthomaticThoughtDiaryRecord();

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
            data.PatientId = patientId;
            data.Sent = model.Sent;

            return data;
        }

        #endregion
    }
}
