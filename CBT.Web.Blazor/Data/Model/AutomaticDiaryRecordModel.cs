using CBT.Web.Blazor.Data.Entities;
using CBT.Web.Blazor.Data.Entities.Enums;
using System.Linq;

namespace CBT.Web.Blazor.Data.Model
{
    public class AutomaticDiaryRecordModel : ThreeColumnsRecordModel // техника трех колонок с. 94
    {
        public string? Situation { get; set; }
        public Dictionary<int, int> BeginningEmotionValues { get; set; }
        public Dictionary<int, int> ResultingEmotionValues { get; set; }

        public List<int> BindEmotionIds { get; set; }

        public AutomaticDiaryRecordModel(Dictionary<int, string>? emotions = null) : base()
        {
            BeginningEmotionValues = emotions?.ToDictionary(x => x.Key, x => 0) ?? new Dictionary<int, int>();
            ResultingEmotionValues = emotions?.ToDictionary(x => x.Key, x => 0) ?? new Dictionary<int, int>();

            BindEmotionIds = new List<int>();
        }


        #region Convert

        public static new AutomaticDiaryRecordModel? Convert(AuthomaticThoughtDiaryRecord data)
        {
            if (data == null)
                return null;

            return new AutomaticDiaryRecordModel()
            {
                Id = data.Id,
                Thought = data.Thought,
                RationalAnswer = data.RationalAnswer,
                Errors = data.ThoughtCognitiveErrors
                    .Select(y => y.CognitiveErrorId).ToList(),

                Situation = data.Situation,
                BeginningEmotionValues = data.ThoughtEmotions
                    .Where(y => y.State == ThoughtEmotionState.Beginning)
                    .ToDictionary(x => x.EmotionId, x => x.Value),
                ResultingEmotionValues = data.ThoughtEmotions
                    .Where(y => y.State == ThoughtEmotionState.Result)
                    .ToDictionary(x => x.EmotionId, x => x.Value),
                BindEmotionIds = data.ThoughtEmotions.Select(x => x.EmotionId).Distinct().ToList()
            };
        }

        #endregion


        #region ConvertBack

        public static AuthomaticThoughtDiaryRecord ConvertBack(AutomaticDiaryRecordModel model, int patientId, AuthomaticThoughtDiaryRecord? data = null)
        {
            data = ThreeColumnsRecordModel.ConvertBack(model, patientId, data);

            data.Situation = model.Situation;
 
            var beginningThoughtEmotionsFiltered = model.BeginningEmotionValues
                    .Where(x => x.Value > 0)
                    .Select(x => ConvertBack(x, ThoughtEmotionState.Beginning, data.Id))
                .ToList();

            var resultingThoughtEmotionsFiltered = model.ResultingEmotionValues
                    .Where(x => beginningThoughtEmotionsFiltered.Any(y => y.EmotionId == x.Key))
                    .Select(x => ConvertBack(x, ThoughtEmotionState.Result, data.Id));

            data.ThoughtEmotions = beginningThoughtEmotionsFiltered.Union(resultingThoughtEmotionsFiltered).ToList();

            return data;
        }

        private static ThoughtEmotion ConvertBack(KeyValuePair<int, int> data, ThoughtEmotionState state, int thoughtId)
        {
            return new ThoughtEmotion
            {
                EmotionId = data.Key,
                Value = data.Value,
                State = state,
                ThoughtId = thoughtId
            };
        }

        #endregion
    }
}
