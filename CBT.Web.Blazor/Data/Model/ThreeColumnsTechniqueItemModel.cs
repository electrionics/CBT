using CBT.Web.Blazor.Data.Entities;

namespace CBT.Web.Blazor.Data.Model
{
    public class ThreeColumnsTechniqueItemModel // техника трех колонок с. 94
    {
        public int Id { get;set; }
        public string Thought { get; set; }
        public List<int> Errors { get; set; }
        public string? RationalAnswer { get; set; }


        public ThreeColumnsTechniqueItemModel() 
        {
            Errors = new List<int>();
        }


        #region Convert

        public static ThreeColumnsTechniqueItemModel? Convert(ThreeColumnsTechnique data)
        {
            if (data == null)
                return null;

            return new ThreeColumnsTechniqueItemModel()
            {
                Id = data.Id,
                Thought = data.Thought,
                RationalAnswer = data.RationalAnswer,
                Errors = data.ThoughtCognitiveErrors.Select(y => y.CognitiveErrorId).ToList()
            };
        }

        #endregion


        #region ConvertBack

        public static ThreeColumnsTechnique ConvertBack(ThreeColumnsTechniqueItemModel model, ThreeColumnsTechnique? data = null)
        {
            if (data == null)
               data = new ThreeColumnsTechnique();

            data.Id = model.Id;
            data.Thought = model.Thought;
            data.RationalAnswer = model.RationalAnswer;
            data.ThoughtCognitiveErrors = model.Errors
                .Select(x => new ThoughtCognitiveError
                {
                    ThoughtId = data.Id,
                    CognitiveErrorId = x
                }).ToList();
            data.UserId = 1;

            return data;
        }

        #endregion
    }
}
