﻿using CBT.Web.Data.Entities;
using CBT.Web.Model.Enums;

namespace CBT.Web.Model
{
    public class ThreeColumnsTechniqueItemModel // техника трех колонок с. 94
    {
        public int Id { get;set; }
        public string Thought { get; set; }
        public List<CognitiveErrors> Errors { get; set; }
        public string? RationalAnswer { get; set; }


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
                Errors = data.ThoughtCognitiveErrors.Select(y => (CognitiveErrors)y.CognitiveErrorId).ToList()
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
                    CognitiveErrorId = (int)x
                })
                .ToList();

            return data;
        }

        #endregion
    }
}
