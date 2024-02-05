using CBT.Web.Blazor.Data.Entities;
using CBT.Web.Blazor.Data;
using CBT.Web.Blazor.Data.Model;
using CBT.Web.Blazor.Data.Model.Enums;
using Microsoft.EntityFrameworkCore;

namespace CBT.Web.Blazor.Services
{
    public class PsychologistReviewService
    {
        private readonly ILogger<PsychologistReviewService> _logger;

        private const string DemoUserId = "DemoPsychologist";

        public PsychologistReviewService(ILogger<PsychologistReviewService> logger)
        {
            _logger = logger;
        }


        #region GetAllRecordReviews

        public async Task<List<ThoughtRecordReview<ThreeColumnsRecordModel>>> GetAllThreeeColumnsRecordReviews(string? userId, ReviewRecordState? filterState)
        {
            using (var dataContext = new CBTDataContext())
            {
                var records = await BuildQuery(dataContext, userId, filterState)
                    .Where(x => !x.Emotions.Any())
                    .ToListAsync();

#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
                return records.Select(ThoughtRecordReview<ThreeColumnsRecordModel>.Convert).ToList();
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
            }
        }

        public async Task<List<ThoughtRecordReview<AutomaticDiaryRecordModel>>> GetAllAutomaticDiaryRecordReviews(string? userId, ReviewRecordState? filterState)
        {
            using (var dataContext = new CBTDataContext())
            {
                var records = await BuildQuery(dataContext, userId, filterState)
                    .Include(x => x.Emotions)
                    .Where(x => x.Emotions.Any())
                    .ToListAsync();

#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
                return records.Select(ThoughtRecordReview<AutomaticDiaryRecordModel>.Convert).ToList();
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
            }
        }

        private IQueryable<AuthomaticThoughtDiaryRecord> BuildQuery(CBTDataContext dataContext, string? userId, ReviewRecordState? filterState)
        {
            var query = dataContext.Set<AuthomaticThoughtDiaryRecord>()
                                    .Include(x => x.CognitiveErrors)
                                    .Include(x => x.PsychologistReviews)
                                    .Include(x => x.Patient)
                                    .AsNoTracking()
                                    .Where(x => x.Patient.Psychologist.UserId == (userId ?? DemoUserId) 
                                                && x.Sent);

            if (filterState == ReviewRecordState.Pending)
            {
                query = query.Where(x => !x.PsychologistReviews.Any());
            }
            else if (filterState == ReviewRecordState.Reviewed)
            {
                query = query.Where(x => x.PsychologistReviews.Any());
            }

            return query;
        }

        #endregion


        #region GetRecordReview

        public async Task<ThoughtRecordReview<ThreeColumnsRecordModel>?> GetThreeColumnRecordReview(int recordId)
        {
            using (var dataContext = new CBTDataContext())
            {
                var record = await dataContext.Set<AuthomaticThoughtDiaryRecord>()
                    .FirstAsync(x => x.Id == recordId);

                return ThoughtRecordReview<ThreeColumnsRecordModel>.Convert(record);
            }
        }

        public async Task<ThoughtRecordReview<AutomaticDiaryRecordModel>?> GetAutomaticDiaryRecordReview(int recordId)
        {
            using (var dataContext = new CBTDataContext())
            {
                var record = await dataContext.Set<AuthomaticThoughtDiaryRecord>()
                    .FirstAsync(x => x.Id == recordId);

                return ThoughtRecordReview<AutomaticDiaryRecordModel>.Convert(record);
            }
        }

        #endregion


        #region SaveRecordReview

        public async Task SaveThreeColumnRecordReview(ThoughtRecordReview<ThreeColumnsRecordModel> model, string? userId)
        {
            using (var dataContext = new CBTDataContext())
            {
                var data = await FetchData(dataContext, model.Value.Id, userId);

                ThoughtRecordReview<ThreeColumnsRecordModel>.ConvertBack(model, data.psychologistId, data.record);

                await dataContext.SaveChangesAsync();
            }
        }

        public async Task SaveAutomaticDiaryRecordReview(ThoughtRecordReview<AutomaticDiaryRecordModel> model, string? userId)
        {
            using (var dataContext = new CBTDataContext())
            {
                var data = await FetchData(dataContext, model.Value.Id, userId);

                model.State = ReviewRecordState.Reviewed;

                ThoughtRecordReview<AutomaticDiaryRecordModel>.ConvertBack(model, data.psychologistId, data.record);

                await dataContext.SaveChangesAsync();
            }
        }

        private async Task<(AuthomaticThoughtDiaryRecord record, int psychologistId)> FetchData(CBTDataContext dataContext, int recordId, string? userId)
        {
            var record = await dataContext.Set<AuthomaticThoughtDiaryRecord>()
                    .Include(x => x.PsychologistReviews)
                    .Include(x => x.CognitiveErrors)
                    .FirstAsync(x => x.Id == recordId);

            var psychologistId = (await dataContext
                .Set<Psychologist>()
                .AsNoTracking()
                .FirstAsync(x => x.UserId == (userId ?? DemoUserId))).Id;

            return (record, psychologistId);
        }

        #endregion


        #region SendRecordReviewToPatient

        public async Task SendRecordToPatient(int id)
        {
            using (var dataContext = new CBTDataContext())
            {
                var data = await dataContext.Set<AuthomaticThoughtDiaryRecord>()
                    .FirstAsync(x => x.Id == id);

                data.SentBack = true;

                await dataContext.SaveChangesAsync();
            }
        }

        #endregion
    }
}
