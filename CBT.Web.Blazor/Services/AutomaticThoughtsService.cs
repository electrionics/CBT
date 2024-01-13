using CBT.Web.Blazor.Data;
using CBT.Web.Blazor.Data.Entities;
using CBT.Web.Blazor.Data.Identity;
using CBT.Web.Blazor.Data.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CBT.Web.Blazor.Services
{
    public class AutomaticThoughtsService
    {
        private readonly ILogger<AutomaticThoughtsService> _logger;
        private readonly UserManager<User> _userManager;

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
                { (int)CognitiveErrors.ExaggerationOrСatastrophization, "Преувеличение" },
                { (int)CognitiveErrors.EmotionalJustification, "Эмоциональное  обоснование" },
                { (int)CognitiveErrors.StatementWithMustWord, "Утверждения со словом \"должен\"" },
                { (int)CognitiveErrors.HangingShortcuts, "Навешивание ярлыков" },
                { (int)CognitiveErrors.Personalization, "Персонализация" },
                { (int)CognitiveErrors.Understatement, "Преуменьшение" },
            };
        }

        #endregion


        #region GetAllEmotions

        public async Task<Dictionary<int, string>> GetAllEmotions()
        {
            using (var dataContext = new CBTDataContext())
            {
                var emotions = await dataContext.Set<Emotion>()
                    .AsNoTracking().ToListAsync();

                return emotions.ToDictionary(x => x.Id, x => x.Name);

                // TODO: sort by frequency
            }
        }

        #endregion


        #region GetAllThoughts

        public async Task<List<ThreeColumnsRecordModel>> GetAllThoughts(string? userId = null)
        {


            using (var dataContext = new CBTDataContext())
            {
                var threeColumnsTechniques = await dataContext.Set<AuthomaticThoughtDiaryRecord>()
                                    .Include(x => x.ThoughtCognitiveErrors)
                                    .AsNoTracking()
                                    .Where(x => !x.ThoughtEmotions.Any())
                                    .Where(x => x.Patient.UserId == (userId ?? "Demo"))
                                    .ToListAsync();
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
                return threeColumnsTechniques
                    .OrderBy(CalculateOrderOfThoughts)
                    .Select(ThreeColumnsRecordModel.Convert)
                    .ToList();
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
            }
        }

        private static int CalculateOrderOfThoughts(AuthomaticThoughtDiaryRecord item)
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
                var data = new AuthomaticThoughtDiaryRecord
                {
                    PatientId = 1,
                    Thought = thought,
                    RationalAnswer = null,
                    ThoughtCognitiveErrors = new List<ThoughtCognitiveError>()
                };

                await dataContext
                    .Set<AuthomaticThoughtDiaryRecord>()
                    .AddAsync(data);
                await dataContext.SaveChangesAsync();

                return data.Id;
            }
        }

        #endregion


        #region AddThoughtFull

        public async Task<int> AddThoughtFull(ThreeColumnsRecordModel model, string? userId = null)
        {
            using (var dataContext = new CBTDataContext())
            {
                var patient = await dataContext.Set<Patient>().FirstAsync(x => x.UserId == (userId ?? "Demo"));

                var data = ThreeColumnsRecordModel.ConvertBack(model, patient.Id);

                await dataContext
                    .Set<AuthomaticThoughtDiaryRecord>()
                    .AddAsync(data);
                await dataContext.SaveChangesAsync();

                return data.Id;
            }
        }

        #endregion


        #region GetThought

        public async Task<ThreeColumnsRecordModel?> GetThought(int id)
        {
            using (var dataContext = new CBTDataContext())
            {
                return ThreeColumnsRecordModel.Convert(await dataContext.Set<AuthomaticThoughtDiaryRecord>()
                    .Include(x => x.ThoughtCognitiveErrors)
                    .AsNoTracking()
                    .FirstAsync(x => x.Id == id));
            }
        }

        #endregion


        #region EditThoughtFull

        public async Task EditThoughtFull(ThreeColumnsRecordModel model, string? userId = null)
        {
            using (var dataContext = new CBTDataContext())
            {
                var patient = await dataContext.Set<Patient>().FirstAsync(x => x.UserId == (userId ?? "Demo"));

                var data = await dataContext.Set<AuthomaticThoughtDiaryRecord>()
                    .Include(x => x.ThoughtCognitiveErrors)
                    .FirstAsync(x => x.Id == model.Id);

                ThreeColumnsRecordModel.ConvertBack(model, patient.Id, data);

                await dataContext.SaveChangesAsync();
            }
        }

        #endregion


        #region DeleteThought

        public async Task DeleteThought(int id)
        {
            using (var dataContext = new CBTDataContext())
            {
                var data = await dataContext.Set<AuthomaticThoughtDiaryRecord>()
                    .Include(x => x.ThoughtCognitiveErrors)
                    .Include(x => x.ThoughtEmotions)
                    .FirstAsync(x => x.Id == id);

                dataContext.Set<AuthomaticThoughtDiaryRecord>()
                    .Remove(data);

                await dataContext.SaveChangesAsync();
            }
        }

        #endregion


        #region GetAllAutomaticThoughts

        public async Task<List<AutomaticDiaryRecordModel>> GetAllAutomaticThoughts(string? userId = null)
        {
            using (var dataContext = new CBTDataContext())
            {
                var threeColumnsTechniques = await dataContext.Set<AuthomaticThoughtDiaryRecord>()
                                    .Include(x => x.ThoughtCognitiveErrors)
                                    .Include(x => x.ThoughtEmotions)
                                    .AsNoTracking()
                                    .Where(x => x.ThoughtEmotions.Any())
                                    .Where(x => x.Patient.UserId == (userId ?? "Demo"))
                                    .ToListAsync();
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
                return threeColumnsTechniques
                    .OrderBy(CalculateOrderOfThoughts)
                    .Select(AutomaticDiaryRecordModel.Convert)
                    .ToList();
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
            }
        }

        #endregion


        #region AddAutomaticThoughtFull

        public async Task<int> AddAutomaticThoughtFull(AutomaticDiaryRecordModel model, string? userId = null)
        {
            using (var dataContext = new CBTDataContext())
            {
                var patient = await dataContext.Set<Patient>().FirstAsync(x => x.UserId == (userId ?? "Demo"));

                var data = AutomaticDiaryRecordModel.ConvertBack(model, patient.Id);

                await dataContext
                    .Set<AuthomaticThoughtDiaryRecord>()
                    .AddAsync(data);
                await dataContext.SaveChangesAsync();

                return data.Id;
            }
        }

        #endregion


        #region GetAutomaticThought

        public async Task<AutomaticDiaryRecordModel?> GetAutomaticThought(int id)
        {
            using (var dataContext = new CBTDataContext())
            {
                return AutomaticDiaryRecordModel.Convert(await dataContext.Set<AuthomaticThoughtDiaryRecord>()
                    .Include(x => x.ThoughtCognitiveErrors)
                    .Include(x => x.ThoughtEmotions)
                    .AsNoTracking()
                    .FirstAsync(x => x.Id == id));
            }
        }

        #endregion


        #region EditAutomaticThoughtFull

        public async Task EditAutomaticThoughtFull(AutomaticDiaryRecordModel model, string? userId = null)
        {
            using (var dataContext = new CBTDataContext())
            {
                var patient = await dataContext.Set<Patient>().FirstAsync(x => x.UserId == (userId ?? "Demo"));

                var data = await dataContext.Set<AuthomaticThoughtDiaryRecord>()
                    .Include(x => x.ThoughtCognitiveErrors).Include(x => x.ThoughtEmotions)
                    .FirstAsync(x => x.Id == model.Id);

                AutomaticDiaryRecordModel.ConvertBack(model, patient.Id, data);

                await dataContext.SaveChangesAsync();
            }
        }

        #endregion
    }
}
