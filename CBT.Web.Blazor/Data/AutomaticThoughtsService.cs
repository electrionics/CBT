using CBT.Web.Blazor.Data.Entities;
using CBT.Web.Blazor.Data.Model;
using Microsoft.EntityFrameworkCore;

namespace CBT.Web.Blazor.Data
{
    public class AutomaticThoughtsService
    {
        private readonly ILogger<AutomaticThoughtsService> _logger;

        public AutomaticThoughtsService(ILogger<AutomaticThoughtsService> logger)
        {
            _logger = logger;
        }

        #region GetAllCognitiveErrors

        public Dictionary<int, string> GetAllCognitiveErrors()
        {
            return new Dictionary<int, string>()
            {
                { (int)CognitiveErrors.AllOrNothing, "Всё или ничего" },
                { (int)CognitiveErrors.Overgenersalization, "Сверхобобщение" },
                { (int)CognitiveErrors.NegativeFilter, "Негативный фильтр" },
                { (int)CognitiveErrors.DepreciationOfPositive, "Обесценивание положительного" },
                { (int)CognitiveErrors.HastyCobnclusions, "Поспешные выводы" },
                { (int)CognitiveErrors.ExaggerationOrСatastrophization, "Катастрофизация (преувеличение)" },
                { (int)CognitiveErrors.EmotionalJustification, "Эмоциональное  обоснование" },
                { (int)CognitiveErrors.StatementWithMustWord, "Утверждения со словом \"должен\"" },
                { (int)CognitiveErrors.HangingShortcuts, "Навешивание ярлыков" },
                { (int)CognitiveErrors.Personalization, "Персонализация" },
                { (int)CognitiveErrors.Understatement, "Преуменьшение" },
            };
        }

        #endregion


        #region GetAllThoughts

        public async Task<List<ThreeColumnsTechniqueItemModel>> GetAllThoughts()
        {
            using (var dataContext = new CBTDataContext())
            {
                var threeColumnsTechniques = await dataContext.Set<ThreeColumnsTechnique>()
                                    .Include(x => x.ThoughtCognitiveErrors)
                                    .AsNoTracking()
                                    .Where(x => x.UserId == 1)
                                    .ToListAsync();
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
                return threeColumnsTechniques
                    .OrderBy(CalculateOrderOfThoughts)
                    .Select(ThreeColumnsTechniqueItemModel.Convert)
                    .ToList();
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
            }
        }

        private static int CalculateOrderOfThoughts(ThreeColumnsTechnique item)
        {
            var orderAddition = 0;
            if (!item.ThoughtCognitiveErrors.Any())
            {
                orderAddition += -2_000_000;
            }
            if (string.IsNullOrEmpty(item.RationalAnswer))
            {
                orderAddition += -1_000_000;
            }

            return orderAddition + item.Id;
        }

        #endregion


        #region AddThought

        public async Task<int> AddThought(string thought)
        {
            using (var dataContext = new CBTDataContext())
            {
                var data = new ThreeColumnsTechnique
                {
                    UserId = 1,
                    Thought = thought,
                    RationalAnswer = null,
                    ThoughtCognitiveErrors = new List<ThoughtCognitiveError>()
                };

                await dataContext
                    .Set<ThreeColumnsTechnique>()
                    .AddAsync(data);
                await dataContext.SaveChangesAsync();

                return data.Id;
            }
        }

        #endregion


        #region AddThoughtFull

        public async Task<int> AddThoughtFull(ThreeColumnsTechniqueItemModel model)
        {
            using (var dataContext = new CBTDataContext())
            {
                var data = ThreeColumnsTechniqueItemModel.ConvertBack(model);

                await dataContext
                    .Set<ThreeColumnsTechnique>()
                    .AddAsync(data);
                await dataContext.SaveChangesAsync();

                return data.Id;
            }
        }

        #endregion


        #region GetThought

        public async Task<ThreeColumnsTechniqueItemModel?> GetThought(int id)
        {
            using (var dataContext = new CBTDataContext())
            {
                return ThreeColumnsTechniqueItemModel.Convert(await dataContext.Set<ThreeColumnsTechnique>()
                    .Include(x => x.ThoughtCognitiveErrors)
                    .AsNoTracking()
                    .FirstAsync(x => x.Id == id));
            }
        }

        #endregion


        #region EditThoughtFull

        public async Task EditThoughtFull(ThreeColumnsTechniqueItemModel model)
        {
            using (var dataContext = new CBTDataContext())
            {
                var data = await dataContext.Set<ThreeColumnsTechnique>()
                    .Include(x => x.ThoughtCognitiveErrors)
                    .FirstAsync(x => x.Id == model.Id);

                ThreeColumnsTechniqueItemModel.ConvertBack(model, data);

                await dataContext.SaveChangesAsync();
            }
        }

        #endregion


        #region DeleteThought

        public async Task DeleteThought(int id)
        {
            using (var dataContext = new CBTDataContext())
            {
                var data = await dataContext.Set<ThreeColumnsTechnique>()
                    .Include(x => x.ThoughtCognitiveErrors)
                    .FirstAsync(x => x.Id == id);

                dataContext.Set<ThreeColumnsTechnique>()
                    .Remove(data);

                await dataContext.SaveChangesAsync();
            }
        }

        #endregion
    }
}
