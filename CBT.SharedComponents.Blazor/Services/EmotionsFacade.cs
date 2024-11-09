using Microsoft.EntityFrameworkCore;

using CBT.Domain;
using CBT.Domain.Entities;
using CBT.SharedComponents.Blazor.Model;
using CBT.Domain.Entities.Enums;

namespace CBT.SharedComponents.Blazor.Services
{
    public class EmotionsFacade(
        CBTDataContextMARS dataContext)
    {
        private readonly CBTDataContextMARS _dataContext = dataContext;

        private const string DemoUserId = "DemoClient";

        #region GetAllEmotions

        public async Task<Dictionary<int, string>> GetAllEmotions()
        {
            var emotions = await _dataContext.Set<Emotion>()
                .AsNoTracking().ToListAsync();

            return emotions.ToDictionary(x => x.Id, x => x.Name);

            // TODO: sort by frequency
        }

        #endregion


        #region GetEmotionsReport

        public async Task<List<EmotionReportItem>> GetEmotionsReport(string? userId = null)
        {
            var allEmotions = await GetAllEmotions();

            var userEmotions = await _dataContext.Set<ThoughtEmotion>()
                .AsNoTracking()
                .Where(x => x.Thought.Patient.UserId == (userId ?? DemoUserId))
                .GroupBy(x => x.EmotionId)
                .ToDictionaryAsync(x => x.Key, x => x.ToList());

            return allEmotions
                .Where(x => userEmotions.ContainsKey(x.Key))
                .Select(x => new EmotionReportItem
                {
                    Name = x.Value,
                    BeforePercent = userEmotions[x.Key]
                        .Where(x => x.State == ThoughtEmotionState.Beginning)
                        .Average(x => (decimal)x.Value),
                    AfterPercent = userEmotions[x.Key]
                        .Where(x => x.State == ThoughtEmotionState.Result)
                        .Average(x => (decimal)x.Value),
                    Count = userEmotions[x.Key].Count(x => x.State == ThoughtEmotionState.Beginning) // no matter which of 2 states to choose
                })
                .ToList();
        }

        #endregion
    }
}
