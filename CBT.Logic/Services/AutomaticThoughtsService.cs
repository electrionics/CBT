using Microsoft.EntityFrameworkCore;

using CBT.Domain;
using CBT.Domain.Entities;
using CBT.Domain.Entities.Enums;
using Microsoft.Extensions.Logging;

namespace CBT.Logic.Services
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

        #region GetAllThoughts

        public async Task<List<AutomaticThought>> GetAllThoughts(string? userId = null)
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

        public async Task<int> AddThoughtFull(AutomaticThought data, string? userId)
        {
            dataContext
                .Set<AutomaticThought>()
                .Add(data);
            await dataContext.SaveChangesAsync();

            return data.Id;
        }

        #endregion


        #region GetThought

        public async Task<AutomaticThought?> GetThought(int id)
        {
            return (await dataContext.Set<AutomaticThought>()
                .Include(x => x.CognitiveErrors)
                .AsNoTracking()
                .FirstAsync(x => x.Id == id));
        }

        #endregion


        #region EditThoughtFull

        public async Task EditThoughtFull(Action<AutomaticThought, int> convertBack, int thoughtId, string? userId)
        {
            var patient = await dataContext.Set<Patient>().FirstAsync(x => x.UserId == (userId ?? DemoUserId));

            var data = await dataContext.Set<AutomaticThought>()
                .Include(x => x.CognitiveErrors)
                .FirstAsync(x => x.Id == thoughtId);

            convertBack(data, patient.Id);

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




        #region GetPsychologistReviews

        public async Task<List<ThoughtPsychologistReview>> GetPsychologistReviews(int thoughtId)
        {
            var data = await dataContext.Set<ThoughtPsychologistReview>().AsNoTracking()
                .Include(x => x.Thought).ThenInclude(x => x.CognitiveErrors)
                .Include(x => x.Psychologist)
                .Where(x => x.SentBack && x.ThoughtId == thoughtId && x.Thought.Sent)
                .OrderByDescending(x => x.DateCreated)
                .ToListAsync();

            return data;
        }

        #endregion


        #region GetAllAutomaticThoughts

        public async Task<List<AutomaticThought>> GetAllAutomaticThoughts(string? userId = null)
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
                .ToList();
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
        }

        #endregion


        #region AddAutomaticThoughtFull

        public async Task<int> AddAutomaticThoughtFull(AutomaticThought data, string? userId)
        {
            var patient = await dataContext.Set<Patient>().FirstAsync(x => x.UserId == (userId ?? DemoUserId));

            dataContext
                .Set<AutomaticThought>()
                .Add(data);
            await dataContext.SaveChangesAsync();

            return data.Id;
        }

        #endregion


        #region GetAutomaticThought

        public async Task<AutomaticThought> GetAutomaticThought(int id)
        {
            return await dataContext.Set<AutomaticThought>()
                .Include(x => x.CognitiveErrors)
                .Include(x => x.Emotions)
                .AsNoTracking()
                .FirstAsync(x => x.Id == id);
        }

        #endregion


        #region EditAutomaticThoughtFull

        public async Task EditAutomaticThoughtFull(Action<AutomaticThought, int> convertBack, int thoughtId, string? userId)
        {
            var patient = await dataContext.Set<Patient>().FirstAsync(x => x.UserId == (userId ?? DemoUserId));

            var data = await dataContext.Set<AutomaticThought>()
                .Include(x => x.CognitiveErrors).Include(x => x.Emotions)
                .FirstAsync(x => x.Id == thoughtId);

            convertBack(data, patient.Id);

            await dataContext.SaveChangesAsync();
        }

        #endregion
    }
}
