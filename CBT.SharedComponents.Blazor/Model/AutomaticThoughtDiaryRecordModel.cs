using CBT.Domain.Entities;
using CBT.Domain.Entities.Enums;
using CBT.SharedComponents.Blazor.Model;

namespace CBT.SharedComponents.Blazor.Model
{
    public class AutomaticThoughtDiaryRecordModel : ThreeColumnsTechniqueRecordModel, IThoughtRecordModel<AutomaticThoughtDiaryRecordModel> // TODO: find in the book!!!
    {
        public string? Situation { get; set; }
        public Dictionary<int, int> BeginningEmotionValues { get; set; }
        public Dictionary<int, int> ResultingEmotionValues { get; set; }

        private List<int> bindEmotionIds = [];
        public List<int> BindEmotionIds 
        { 
            get
            {
                return bindEmotionIds;
            }
            set
            {
                if (value != null)
                {
                    bindEmotionIds = value;
                    return;
                }

                bindEmotionIds = [];
            }
        }

        public AutomaticThoughtDiaryRecordModel(Dictionary<int, string> emotions) : this()
        {
            foreach (var kvp in emotions.Select(x => new KeyValuePair<int, int>(x.Key, 0)))
            {
                BeginningEmotionValues.Add(kvp.Key, kvp.Value);
                ResultingEmotionValues.Add(kvp.Key, kvp.Value);
            }
        }

        public AutomaticThoughtDiaryRecordModel() : base()
        {
            BeginningEmotionValues = new Dictionary<int, int>();
            ResultingEmotionValues = new Dictionary<int, int>();

            BindEmotionIds = new List<int>();
        }


        #region Convert

        public new AutomaticThoughtDiaryRecordModel? Convert(AutomaticThought data)
        {
            if (data == null)
                return null;

            return new AutomaticThoughtDiaryRecordModel()
            {
                Id = data.Id,
                Thought = data.Thought,
                RationalAnswer = data.RationalAnswer,
                Errors = data.CognitiveErrors
                    .Select(y => y.CognitiveErrorId).ToList(),
                Sent = data.Sent,

                Situation = data.Situation,
                BeginningEmotionValues = data.Emotions
                    .Where(y => y.State == ThoughtEmotionState.Beginning)
                    .ToDictionary(x => x.EmotionId, x => x.Value),
                ResultingEmotionValues = data.Emotions
                    .Where(y => y.State == ThoughtEmotionState.Result)
                    .ToDictionary(x => x.EmotionId, x => x.Value),
                BindEmotionIds = data.Emotions.Select(x => x.EmotionId).Distinct().ToList()
            };
        }

        #endregion


        #region ConvertBack

        public new AutomaticThought ConvertBack(int patientId, DiaryType type, AutomaticThought? data = null)
        {
            var model = this;

            data = base.ConvertBack(patientId, type, data);

            data.Situation = model.Situation;
 
            var beginningThoughtEmotionsFiltered = model.BeginningEmotionValues
                    .Where(x => x.Value > 0)
                    .Select(x => ConvertBack(x, ThoughtEmotionState.Beginning, data.Id))
                .ToList();

            var resultingThoughtEmotionsFiltered = model.ResultingEmotionValues
                    .Where(x => beginningThoughtEmotionsFiltered.Any(y => y.EmotionId == x.Key))
                    .Select(x => ConvertBack(x, ThoughtEmotionState.Result, data.Id));

            data.Emotions = beginningThoughtEmotionsFiltered.Union(resultingThoughtEmotionsFiltered).ToList();

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
