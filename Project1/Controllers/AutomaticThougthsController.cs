using CBT.Web.Data;
using CBT.Web.Data.Entities;
using CBT.Web.Model;
using CBT.Web.Model.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CBT.Web.Controllers
{
    [ApiController]
    public class AutomaticThougthsController : ControllerBase
    {
        private readonly ILogger<AutomaticThougthsController> _logger;

        public AutomaticThougthsController(ILogger<AutomaticThougthsController> logger)
        {
            _logger = logger;
        }

        #region GetAllCognitiveErrors

        [HttpGet]
        [Route("/AutomaticThougths/GetAllCognitiveErrors")]
        public IEnumerable<KeyValuePair<int, string>> GetAllCognitiveErrors()
        {
            return new List<KeyValuePair<int, string>>()
            {
                new ((int)CognitiveErrors.AllOrNothing, "Всё или ничего"),
                new ((int)CognitiveErrors.Overgenersalization, "Сверхобобщение"),
                new ((int)CognitiveErrors.NegativeFilter, "Негативный фильтр"),
                new ((int)CognitiveErrors.DepreciationOfPositive, "Обесценивание положительного"),
                new ((int)CognitiveErrors.HastyCobnclusions, "Катастрофизавция (преивелоичение)"),
                new ((int)CognitiveErrors.ExaggerationOrСatastrophization, "Всё или ничего"),
                new ((int)CognitiveErrors.EmotionalJustification, "Эмоциональное  обоснование"),
                new ((int)CognitiveErrors.StatementWithMustWord, "Утверждения со словом должен"),
                new ((int)CognitiveErrors.HangingShortcuts, "Навешивание ярлыков"),
                new ((int)CognitiveErrors.Personalization, "Персонализация"),
                new ((int)CognitiveErrors.Understatement, "Преуменьшение"),
            };
        }

        #endregion


        #region GetAllThoughts

        [HttpGet]
        [Route("/AutomaticThougths/GetAllThoughts")]
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

        [HttpPost]
        [Route("/AutomaticThougths/AddThought")]
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

        [HttpPost]
        [Route("/AutomaticThougths/AddThoughtFull")]
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

        [HttpGet]
        [Route("/AutomaticThougths/GetThought")]
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

        [HttpPost]
        [Route("/AutomaticThougths/EditThoughtFull")]
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