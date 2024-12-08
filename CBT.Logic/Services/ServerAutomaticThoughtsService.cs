using Microsoft.EntityFrameworkCore;

using CBT.Domain;
using CBT.Domain.Entities;
using CBT.Domain.Entities.Enums;
using CBT.Logic.Contracts;

namespace CBT.Logic.Services
{
    public class ServerAutomaticThoughtsService(
        CBTDataContext dataContext): IAutomaticThoughtsService
    {
        private readonly CBTDataContext _dataContext = dataContext;

        private const string DemoUserId = "DemoClient";

        #region GetAllThoughts

        public async Task<List<AutomaticThought>> GetAllThoughts(string? userId = null)
        {
            var threeColumnsTechniques = await _dataContext.Set<AutomaticThought>()
                                .Include(x => x.CognitiveErrors)
                                .AsNoTracking()
                                .Where(x => x.Type == DiaryType.ThreeColumnsTechnique)
                                .Where(x => x.Patient.UserId == (userId ?? DemoUserId))
                                .ToListAsync();
            return [.. threeColumnsTechniques.OrderBy(CalculateOrderOfThoughts)];
        }

        private static int CalculateOrderOfThoughts(AutomaticThought item)
        {
            var orderAddition = 0;
            if (item.CognitiveErrors.Count == 0)
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
                CognitiveErrors = []
            };

            await _dataContext
                .Set<AutomaticThought>()
                .AddAsync(data);
            await _dataContext.SaveChangesAsync();

            return data.Id;
        }

        #endregion


        #region AddThoughtFull

        public async Task<int> AddThoughtFull(AutomaticThought data)
        {
            _dataContext
                .Set<AutomaticThought>()
                .Add(data);
            await _dataContext.SaveChangesAsync();

            return data.Id;
        }

        #endregion


        #region GetThought

        public async Task<AutomaticThought?> GetThought(int id)
        {
            return (await _dataContext.Set<AutomaticThought>()
                .Include(x => x.CognitiveErrors)
                .AsNoTracking()
                .FirstAsync(x => x.Id == id));
        }

        #endregion


        #region EditThoughtFull

        public async Task EditThoughtFull(Action<AutomaticThought, int> convertBack, int thoughtId, string? userId)
        {
            var patient = await _dataContext.Set<Patient>().FirstAsync(x => x.UserId == (userId ?? DemoUserId));

            var data = await _dataContext.Set<AutomaticThought>()
                .Include(x => x.CognitiveErrors)
                .FirstAsync(x => x.Id == thoughtId);

            convertBack(data, patient.Id);

            await _dataContext.SaveChangesAsync();
        }

        #endregion


        #region DeleteThought

        public async Task DeleteThought(int id)
        {
            var data = await _dataContext.Set<AutomaticThought>()
                .Include(x => x.CognitiveErrors)
                .Include(x => x.Emotions)
                .FirstAsync(x => x.Id == id);

            _dataContext.Set<AutomaticThought>()
                .Remove(data);

            await _dataContext.SaveChangesAsync();
        }

        #endregion


        #region SendThoughtToPsychologist

        public async Task SendThoughtToPsychologist(int id)
        {
            var data = await _dataContext.Set<AutomaticThought>()
                .FirstAsync(x => x.Id == id);

            data.Sent = true;

            await _dataContext.SaveChangesAsync();
        }

        #endregion




        #region GetPsychologistReviews

        public async Task<List<ThoughtPsychologistReview>> GetPsychologistReviews(int thoughtId)
        {
            var data = await _dataContext.Set<ThoughtPsychologistReview>().AsNoTracking()
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
            var threeColumnsTechniques = await _dataContext.Set<AutomaticThought>()
                                .Include(x => x.CognitiveErrors)
                                .Include(x => x.Emotions).ThenInclude(x => x.Emotion)
                                .AsNoTracking()
                                .Where(x => x.Type == DiaryType.AutomaticThoughtDiary)
                                .Where(x => x.Patient.UserId == (userId ?? DemoUserId))
                                .ToListAsync();
            return [.. threeColumnsTechniques.OrderBy(CalculateOrderOfThoughts)];
        }

        #endregion


        #region AddAutomaticThoughtFull

        public async Task<int> AddAutomaticThoughtFull(AutomaticThought data, string? userId)
        {
            var patient = await _dataContext.Set<Patient>().FirstAsync(x => x.UserId == (userId ?? DemoUserId));

            _dataContext
                .Set<AutomaticThought>()
                .Add(data);
            await _dataContext.SaveChangesAsync();

            return data.Id;
        }

        #endregion


        #region GetAutomaticThought

        public async Task<AutomaticThought> GetAutomaticThought(int id)
        {
            return await _dataContext.Set<AutomaticThought>()
                .Include(x => x.CognitiveErrors)
                .Include(x => x.Emotions).ThenInclude(x => x.Emotion)
                .AsNoTracking()
                .FirstAsync(x => x.Id == id);
        }

        #endregion


        #region EditAutomaticThoughtFull

        public async Task EditAutomaticThoughtFull(Action<AutomaticThought, int> convertBack, int thoughtId, string? userId)
        {
            var patient = await _dataContext.Set<Patient>().FirstAsync(x => x.UserId == (userId ?? DemoUserId));

            var data = await _dataContext.Set<AutomaticThought>()
                .Include(x => x.CognitiveErrors).Include(x => x.Emotions)
                .FirstAsync(x => x.Id == thoughtId);

            convertBack(data, patient.Id);

            await _dataContext.SaveChangesAsync();
        }

        #endregion
    }
}
