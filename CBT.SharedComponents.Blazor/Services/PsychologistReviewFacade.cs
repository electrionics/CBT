using Microsoft.EntityFrameworkCore;

using CBT.Domain;
using CBT.Domain.Entities;
using CBT.Domain.Entities.Enums;
using CBT.SharedComponents.Blazor.Model;
using CBT.SharedComponents.Blazor.Model.Enums;

namespace CBT.SharedComponents.Blazor.Services
{
    public class PsychologistReviewFacade(
        CBTDataContext dataContext)
    {
        private readonly CBTDataContext _dataContext = dataContext;

        private const string DemoUserId = "DemoPsychologist";


        #region GetAllRecordReviews

        public async Task<List<ThoughtRecordReview<ThreeColumnsTechniqueRecordModel>>> GetAllThreeeColumnsRecordReviews(string? userId, ReviewRecordState? filterState)
        {
            var psychologist = await _dataContext.Set<Psychologist>().AsNoTracking()
                .FirstAsync(x => x.UserId == userId);

            var records = await BuildQuery(_dataContext, userId, filterState)
                    .Where(x => x.Type == DiaryType.ThreeColumnsTechnique)
                    .ToListAsync();

#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
            return records
                .Select(record => ThoughtRecordReview<ThreeColumnsTechniqueRecordModel>.Convert(record, psychologist.Id))
                .ToList();
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
        }

        public async Task<List<ThoughtRecordReview<AutomaticThoughtDiaryRecordModel>>> GetAllAutomaticDiaryRecordReviews(string? userId, ReviewRecordState? filterState)
        {
            var psychologist = await _dataContext.Set<Psychologist>().AsNoTracking()
                .FirstAsync(x => x.UserId == userId);

            var records = await BuildQuery(_dataContext, userId, filterState)
                    .Include(x => x.Emotions).ThenInclude(x => x.Emotion)
                    .Where(x => x.Type == DiaryType.AutomaticThoughtDiary)
                    .ToListAsync();

#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
            return records
                .Select(record => ThoughtRecordReview<AutomaticThoughtDiaryRecordModel>.Convert(record, psychologist.Id))
                .ToList();
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
        }

        private static IQueryable<AutomaticThought> BuildQuery(CBTDataContext dataContextParameter, string? userId, ReviewRecordState? filterState)
        {
            var query = dataContextParameter.Set<AutomaticThought>()
                                    .Include(x => x.CognitiveErrors)
                                    .Include(x => x.PsychologistReviews)
                                    .Include(x => x.Patient)
                                    .AsNoTracking()
                                    .Where(x => x.Patient.Psychologists.Any(y => y.Psychologist.UserId == (userId ?? DemoUserId))
                                                && x.Sent);

            if (filterState == ReviewRecordState.Pending)
            {
                query = query.Where(x => x.PsychologistReviews.Count == 0);
            }
            else if (filterState == ReviewRecordState.Reviewed)
            {
                query = query.Where(x => x.PsychologistReviews.Count != 0);
            }

            return query;
        }

        #endregion


        #region GetRecordReview

        public async Task<ThoughtRecordReview<ThreeColumnsTechniqueRecordModel>?> GetThreeColumnRecordReview(int recordId, string? userId)
        {
            var psychologist = await _dataContext.Set<Psychologist>().AsNoTracking()
                .FirstAsync(x => x.UserId == userId);

            var record = await _dataContext.Set<AutomaticThought>()
                .Include(x => x.PsychologistReviews)
                .Include(x => x.CognitiveErrors)
                .Include(x => x.Patient)
                .FirstAsync(x => x.Id == recordId);

            return ThoughtRecordReview<ThreeColumnsTechniqueRecordModel>.Convert(record, psychologist.Id);
        }

        public async Task<ThoughtRecordReview<AutomaticThoughtDiaryRecordModel>?> GetAutomaticDiaryRecordReview(int recordId, string? userId)
        {
            var psychologist = await _dataContext.Set<Psychologist>().AsNoTracking()
                .FirstAsync(x => x.UserId == userId);

            var record = await _dataContext.Set<AutomaticThought>()
                .Include(x => x.PsychologistReviews)
                .Include(x => x.CognitiveErrors)
                .Include(x => x.Patient)
                .Include(x => x.Emotions).ThenInclude(x => x.Emotion)
                .FirstAsync(x => x.Id == recordId);

            return ThoughtRecordReview<AutomaticThoughtDiaryRecordModel>.Convert(record, psychologist.Id);
        }

        #endregion


        #region SaveRecordReview

        public async Task SaveThreeColumnRecordReview(ThoughtRecordReview<ThreeColumnsTechniqueRecordModel> model, string? userId)
        {
            var (record, psychologistId) = await FetchData(_dataContext, model.Value.Id, userId); 
            
            model.State = ReviewRecordState.Reviewed;

            ThoughtRecordReview<ThreeColumnsTechniqueRecordModel>.ConvertBack(model, psychologistId, record);

            await _dataContext.SaveChangesAsync();
        }

        public async Task SaveAutomaticDiaryRecordReview(ThoughtRecordReview<AutomaticThoughtDiaryRecordModel> model, string? userId)
        {
            var (record, psychologistId) = await FetchData(_dataContext, model.Value.Id, userId);

            model.State = ReviewRecordState.Reviewed;

            ThoughtRecordReview<AutomaticThoughtDiaryRecordModel>.ConvertBack(model, psychologistId, record);

            await _dataContext.SaveChangesAsync();
        }

        private static async Task<(AutomaticThought record, int psychologistId)> FetchData(CBTDataContext dataContextParameter, int recordId, string? userId)
        {
            var record = await dataContextParameter.Set<AutomaticThought>()
                    .Include(x => x.PsychologistReviews)
                    .Include(x => x.CognitiveErrors)
                    .FirstAsync(x => x.Id == recordId);

            var psychologistId = (await dataContextParameter
                .Set<Psychologist>()
                .AsNoTracking()
                .FirstAsync(x => x.UserId == (userId ?? DemoUserId))).Id;

            return (record, psychologistId);
        }

        #endregion


        #region SendRecordReviewToPatient

        public async Task SendRecordToPatient(int thoughtId)
        {
            var data = await _dataContext.Set<ThoughtPsychologistReview>()
                    .FirstAsync(x => x.ThoughtId == thoughtId);

            data.SentBack = true;

            await _dataContext.SaveChangesAsync();
        }

        #endregion
    }
}
