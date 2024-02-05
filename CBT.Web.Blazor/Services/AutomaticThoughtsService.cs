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

        private const string DemoUserId = "DemoClient";

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
                                    .Include(x => x.CognitiveErrors)
                                    .AsNoTracking()
                                    .Where(x => !x.Emotions.Any())
                                    .Where(x => x.Patient.UserId == (userId ?? DemoUserId))
                                    .ToListAsync();
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
                return threeColumnsTechniques
                    .OrderBy(CalculateOrderOfThoughts)
                    .Select(new ThreeColumnsRecordModel().Convert)
                    .ToList();
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
            }
        }

        private static int CalculateOrderOfThoughts(AuthomaticThoughtDiaryRecord item)
        {
            var orderAddition = 0;
            if (!item.CognitiveErrors.Any())
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
                    CognitiveErrors = new List<ThoughtCognitiveError>()
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

        public async Task<int> AddThoughtFull(ThreeColumnsRecordModel model, string? userId)
        {
            using (var dataContext = new CBTDataContext())
            {
                var patient = await dataContext.Set<Patient>().FirstAsync(x => x.UserId == (userId ?? DemoUserId));

                var data = model.ConvertBack(patient.Id);

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
                return new ThreeColumnsRecordModel().Convert(await dataContext.Set<AuthomaticThoughtDiaryRecord>()
                    .Include(x => x.CognitiveErrors)
                    .AsNoTracking()
                    .FirstAsync(x => x.Id == id));
            }
        }

        #endregion


        #region EditThoughtFull

        public async Task EditThoughtFull(ThreeColumnsRecordModel model, string? userId)
        {
            using (var dataContext = new CBTDataContext())
            {
                var patient = await dataContext.Set<Patient>().FirstAsync(x => x.UserId == (userId ?? DemoUserId));

                var data = await dataContext.Set<AuthomaticThoughtDiaryRecord>()
                    .Include(x => x.CognitiveErrors)
                    .FirstAsync(x => x.Id == model.Id);

                model.ConvertBack(patient.Id, data);

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
                    .Include(x => x.CognitiveErrors)
                    .Include(x => x.Emotions)
                    .FirstAsync(x => x.Id == id);

                dataContext.Set<AuthomaticThoughtDiaryRecord>()
                    .Remove(data);

                await dataContext.SaveChangesAsync();
            }
        }

        #endregion


        #region SendThoughtToPsychologist

        public async Task SendThoughtToPsychologist(int id)
        {
            using (var dataContext = new CBTDataContext())
            {
                var data = await dataContext.Set<AuthomaticThoughtDiaryRecord>()
                    .FirstAsync(x => x.Id == id);

                data.Sent = true;

                await dataContext.SaveChangesAsync();
            }
        }

        #endregion




        #region GetPsychologistReview

        public async Task<RecordReviewModel?> GetPsychologistReview(int id)
        {
            using (var dataContext = new CBTDataContext())
            {
                var data = await dataContext.Set<ThoughtPsychologistReview>()
                    .Include(x => x.Thought).ThenInclude(x => x.CognitiveErrors)
                    .FirstOrDefaultAsync(x => x.ThoughtId == id && x.Thought.Sent && x.Thought.SentBack);

                return data == null ? null : new()
                {
                    Id = data.ThoughtId,
                    RationalAnswerComment = data.RationalAnswerComment,
                    ReviewedErrors = data.Thought.CognitiveErrors
                        .Where(x => x.IsReview && x.PsychologistId == data.PsychologistId)
                        .Select(x => x.CognitiveErrorId)
                        .ToList(),
                };
            }
        }

        #endregion


        #region GetAllAutomaticThoughts

        public async Task<List<AutomaticDiaryRecordModel>> GetAllAutomaticThoughts(string? userId = null)
        {
            using (var dataContext = new CBTDataContext())
            {
                var threeColumnsTechniques = await dataContext.Set<AuthomaticThoughtDiaryRecord>()
                                    .Include(x => x.CognitiveErrors)
                                    .Include(x => x.Emotions)
                                    .AsNoTracking()
                                    .Where(x => x.Emotions.Any())
                                    .Where(x => x.Patient.UserId == (userId ?? DemoUserId))
                                    .ToListAsync();
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
                return threeColumnsTechniques
                    .OrderBy(CalculateOrderOfThoughts)
                    .Select(new AutomaticDiaryRecordModel().Convert)
                    .ToList();
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
            }
        }

        #endregion


        #region AddAutomaticThoughtFull

        public async Task<int> AddAutomaticThoughtFull(AutomaticDiaryRecordModel model, string? userId)
        {
            using (var dataContext = new CBTDataContext())
            {
                var patient = await dataContext.Set<Patient>().FirstAsync(x => x.UserId == (userId ?? DemoUserId));

                var data = model.ConvertBack(patient.Id);

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
                return new AutomaticDiaryRecordModel().Convert(await dataContext.Set<AuthomaticThoughtDiaryRecord>()
                    .Include(x => x.CognitiveErrors)
                    .Include(x => x.Emotions)
                    .AsNoTracking()
                    .FirstAsync(x => x.Id == id));
            }
        }

        #endregion


        #region EditAutomaticThoughtFull

        public async Task EditAutomaticThoughtFull(AutomaticDiaryRecordModel model, string? userId)
        {
            using (var dataContext = new CBTDataContext())
            {
                var patient = await dataContext.Set<Patient>().FirstAsync(x => x.UserId == (userId ?? DemoUserId));

                var data = await dataContext.Set<AuthomaticThoughtDiaryRecord>()
                    .Include(x => x.CognitiveErrors).Include(x => x.Emotions)
                    .FirstAsync(x => x.Id == model.Id);

                model.ConvertBack(patient.Id, data);

                await dataContext.SaveChangesAsync();
            }
        }

        #endregion
    }
}
