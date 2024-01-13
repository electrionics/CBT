using CBT.Web.Blazor.Data.Entities;

namespace CBT.Web.Blazor.Data.Model
{
    public class ThreeColumnsRecordModel // техника трех колонок с. 94
    {
        public int Id { get;set; }

        public string Thought { get; set; }
        public List<int> Errors { get; set; }
        public string? RationalAnswer { get; set; }


        public ThreeColumnsRecordModel() 
        {
            Errors = new List<int>();
        }


        #region Convert

        public static ThreeColumnsRecordModel? Convert(AuthomaticThoughtDiaryRecord data)
        {
            if (data == null)
                return null;

            return new ThreeColumnsRecordModel()
            {
                Id = data.Id,
                Thought = data.Thought,
                RationalAnswer = data.RationalAnswer,
                Errors = data.ThoughtCognitiveErrors.Select(y => y.CognitiveErrorId).ToList()
            };
        }

        #endregion


        #region ConvertBack

        public static AuthomaticThoughtDiaryRecord ConvertBack(ThreeColumnsRecordModel model, int patientId, AuthomaticThoughtDiaryRecord? data = null)
        {
            if (data == null)
               data = new AuthomaticThoughtDiaryRecord();

            data.Id = model.Id;
            data.Thought = model.Thought;
            data.RationalAnswer = model.RationalAnswer;
            data.ThoughtCognitiveErrors = model.Errors
                .Select(x => new ThoughtCognitiveError
                {
                    ThoughtId = data.Id,
                    CognitiveErrorId = x
                }).ToList();
            data.ThoughtEmotions = new List<ThoughtEmotion>();
            data.PatientId = patientId;

            return data;
        }

        #endregion
    }
}
