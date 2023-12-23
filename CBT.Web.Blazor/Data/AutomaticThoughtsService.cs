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

        public IEnumerable<KeyValuePair<int, string>> GetAllCognitiveErrors()
        {
            return new List<KeyValuePair<int, string>>()
            {
                new ((int)CognitiveErrors.AllOrNothing, "Всё или ничего"),
                new ((int)CognitiveErrors.Overgenersalization, "Сверхобобщение"),
                new ((int)CognitiveErrors.NegativeFilter, "Негативный фильтр"),
                new ((int)CognitiveErrors.DepreciationOfPositive, "Обесценивание положительного"),
                new ((int)CognitiveErrors.HastyCobnclusions, "Поспешные выводы"),
                new ((int)CognitiveErrors.ExaggerationOrСatastrophization, "Катастрофизавция (преувеличение)"),
                new ((int)CognitiveErrors.EmotionalJustification, "Эмоциональное  обоснование"),
                new ((int)CognitiveErrors.StatementWithMustWord, "Утверждения со словом должен"),
                new ((int)CognitiveErrors.HangingShortcuts, "Навешивание ярлыков"),
                new ((int)CognitiveErrors.Personalization, "Персонализация"),
                new ((int)CognitiveErrors.Understatement, "Преуменьшение"),
            };
        }

        #endregion


        #region GetAllThoughts

        public async Task<List<ThreeColumnsTechniqueItemModel?>> GetAllThoughts()
        {
            using (var dataContext = new CBTDataContext())
            {
                return (await dataContext.Set<ThreeColumnsTechnique>()
                    .Include(x => x.ThoughtCognitiveErrors)
                    .AsNoTracking()
                    .Where(x => x.UserId == 1)
                    .ToListAsync())
                    .Select(ThreeColumnsTechniqueItemModel.Convert)
                    .ToList();
            }
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
                var data = await dataContext.Set<ThreeColumnsTechnique>().FirstAsync(x => x.Id == model.Id);

                ThreeColumnsTechniqueItemModel.ConvertBack(model, data);

                await dataContext.SaveChangesAsync();
            }
        }

        #endregion
    }
}
