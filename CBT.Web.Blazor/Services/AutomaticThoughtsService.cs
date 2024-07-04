using Microsoft.EntityFrameworkCore;

using CBT.Web.Blazor.Data;
using CBT.Web.Blazor.Data.Entities;
using CBT.Web.Blazor.Data.Entities.Enums;
using CBT.Web.Blazor.Data.Model;

namespace CBT.Web.Blazor.Services
{
    public class AutomaticThoughtsService
    {
        private readonly ILogger<AutomaticThoughtsService> _logger; //TODO: use logger in this class
        private readonly CBTDataContext dataContext;
        private const string DemoUserId = "DemoClient";

        public AutomaticThoughtsService(ILogger<AutomaticThoughtsService> logger, CBTDataContext dataContext)
        {
            _logger = logger;
            this.dataContext = dataContext;
        }


        #region GetAllEmotions

        public async Task<Dictionary<int, string>> GetAllEmotions()
        {
            var emotions = await dataContext.Set<Emotion>()
                .AsNoTracking().ToListAsync();

            return emotions.ToDictionary(x => x.Id, x => x.Name);

            // TODO: sort by frequency
        }

        #endregion


        #region GetAllThoughts

        public async Task<List<ThreeColumnsTechniqueRecordModel>> GetAllThoughts(string? userId = null)
        {
            var threeColumnsTechniques = await dataContext.Set<AutomaticThought>()
                                .Include(x => x.CognitiveErrors)
                                .AsNoTracking()
                                .Where(x => x.Type == DiaryType.ThreeColumnsTechnique)
                                .Where(x => x.Patient.UserId == (userId ?? DemoUserId))
                                .ToListAsync();
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
            return threeColumnsTechniques
                .OrderBy(CalculateOrderOfThoughts)
                .Select(new ThreeColumnsTechniqueRecordModel().Convert)
                .ToList();
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
        }

        private static int CalculateOrderOfThoughts(AutomaticThought item)
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
            var data = new AutomaticThought
            {
                PatientId = 1,
                Thought = thought,
                RationalAnswer = null,
                Type = DiaryType.ThreeColumnsTechnique,
                CognitiveErrors = new List<ThoughtCognitiveError>()
            };

            await dataContext
                .Set<AutomaticThought>()
                .AddAsync(data);
            await dataContext.SaveChangesAsync();

            return data.Id;
        }

        #endregion


        #region AddThoughtFull

        public async Task<int> AddThoughtFull(ThreeColumnsTechniqueRecordModel model, string? userId)
        {
            var patient = await dataContext.Set<Patient>().FirstAsync(x => x.UserId == (userId ?? DemoUserId));

            var data = model.ConvertBack(patient.Id, DiaryType.ThreeColumnsTechnique);

            await dataContext
                .Set<AutomaticThought>()
                .AddAsync(data);
            await dataContext.SaveChangesAsync();

            return data.Id;
        }

        #endregion


        #region GetThought

        public async Task<ThreeColumnsTechniqueRecordModel?> GetThought(int id)
        {
            return new ThreeColumnsTechniqueRecordModel().Convert(await dataContext.Set<AutomaticThought>()
                .Include(x => x.CognitiveErrors)
                .AsNoTracking()
                .FirstAsync(x => x.Id == id));
        }

        #endregion


        #region EditThoughtFull

        public async Task EditThoughtFull(ThreeColumnsTechniqueRecordModel model, string? userId)
        {
            var patient = await dataContext.Set<Patient>().FirstAsync(x => x.UserId == (userId ?? DemoUserId));

            var data = await dataContext.Set<AutomaticThought>()
                .Include(x => x.CognitiveErrors)
                .FirstAsync(x => x.Id == model.Id);

            model.ConvertBack(patient.Id, DiaryType.ThreeColumnsTechnique, data);

            await dataContext.SaveChangesAsync();
        }

        #endregion


        #region DeleteThought

        public async Task DeleteThought(int id)
        {
            var data = await dataContext.Set<AutomaticThought>()
                .Include(x => x.CognitiveErrors)
                .Include(x => x.Emotions)
                .FirstAsync(x => x.Id == id);

            dataContext.Set<AutomaticThought>()
                .Remove(data);

            await dataContext.SaveChangesAsync();
        }

        #endregion


        #region SendThoughtToPsychologist

        public async Task SendThoughtToPsychologist(int id)
        {
            var data = await dataContext.Set<AutomaticThought>()
                .FirstAsync(x => x.Id == id);

            data.Sent = true;

            await dataContext.SaveChangesAsync();
        }

        #endregion




        #region GetPsychologistReview

        public async Task<RecordReviewModel?> GetPsychologistReview(int id)
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

        #endregion


        #region GetAllAutomaticThoughts

        public async Task<List<AutomaticThoughtDiaryRecordModel>> GetAllAutomaticThoughts(string? userId = null)
        {
            var threeColumnsTechniques = await dataContext.Set<AutomaticThought>()
                                .Include(x => x.CognitiveErrors)
                                .Include(x => x.Emotions)
                                .AsNoTracking()
                                .Where(x => x.Type == DiaryType.AutomaticThoughtDiary)
                                .Where(x => x.Patient.UserId == (userId ?? DemoUserId))
                                .ToListAsync();
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
            return threeColumnsTechniques
                .OrderBy(CalculateOrderOfThoughts)
                .Select(new AutomaticThoughtDiaryRecordModel().Convert)
                .ToList();
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
        }

        #endregion


        #region AddAutomaticThoughtFull

        public async Task<int> AddAutomaticThoughtFull(AutomaticThoughtDiaryRecordModel model, string? userId)
        {
            var patient = await dataContext.Set<Patient>().FirstAsync(x => x.UserId == (userId ?? DemoUserId));

            var data = model.ConvertBack(patient.Id, DiaryType.AutomaticThoughtDiary);

            await dataContext
                .Set<AutomaticThought>()
                .AddAsync(data);
            await dataContext.SaveChangesAsync();

            return data.Id;
        }

        #endregion


        #region GetAutomaticThought

        public async Task<AutomaticThoughtDiaryRecordModel?> GetAutomaticThought(int id)
        {
            return new AutomaticThoughtDiaryRecordModel().Convert(await dataContext.Set<AutomaticThought>()
                .Include(x => x.CognitiveErrors)
                .Include(x => x.Emotions)
                .AsNoTracking()
                .FirstAsync(x => x.Id == id));
        }

        #endregion


        #region EditAutomaticThoughtFull

        public async Task EditAutomaticThoughtFull(AutomaticThoughtDiaryRecordModel model, string? userId)
        {
            var patient = await dataContext.Set<Patient>().FirstAsync(x => x.UserId == (userId ?? DemoUserId));

            var data = await dataContext.Set<AutomaticThought>()
                .Include(x => x.CognitiveErrors).Include(x => x.Emotions)
                .FirstAsync(x => x.Id == model.Id);

            model.ConvertBack(patient.Id, DiaryType.AutomaticThoughtDiary, data);

            await dataContext.SaveChangesAsync();
        }

        #endregion
    }
}
