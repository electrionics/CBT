using Microsoft.EntityFrameworkCore;

using CBT.Domain;
using CBT.Domain.Entities;
using CBT.Logic.Contracts;

namespace CBT.Logic.Services
{
    public class ServerMoodRecordService(CBTDataContext dataContext) : IMoodRecordService
    {
        private readonly CBTDataContext _dataContext = dataContext;

        private const string DemoUserId = "DemoClient";


        #region GetAllMoodRecords

        public async Task<List<MoodRecord>> GetAllMoodRecords(string? userId = null)
        {
            var records = await _dataContext.Set<MoodRecord>()
                                .AsNoTracking()
                                .Where(x => x.Patient.UserId == (userId ?? DemoUserId))
                                .ToListAsync();

            return [.. records.OrderBy(CalculateOrderOfRecords)];
        }

        private static long CalculateOrderOfRecords(MoodRecord item)
        {
            return item.DateTime.Ticks;
        }

        #endregion


        #region AddMoodRecordFull

        public async Task<int> AddMoodRecordFull(MoodRecord data, string? userId)
        {
            var patient = await _dataContext.Set<Patient>().FirstAsync(x => x.UserId == (userId ?? DemoUserId));

            _dataContext
                .Set<MoodRecord>()
                .Add(data);
            await _dataContext.SaveChangesAsync();

            return data.Id;
        }

        #endregion


        #region GetMoodRecord

        public async Task<MoodRecord> GetMoodRecord(int id)
        {
            return await _dataContext.Set<MoodRecord>()
                .AsNoTracking()
                .FirstAsync(x => x.Id == id);
        }

        #endregion


        #region EditMoodRecordFull

        public async Task EditMoodRecordFull(Action<MoodRecord, int> convertBack, int recordId, string? userId)
        {
            var patient = await _dataContext.Set<Patient>().FirstAsync(x => x.UserId == (userId ?? DemoUserId));

            var data = await _dataContext.Set<MoodRecord>()
                .FirstAsync(x => x.Id == recordId);

            convertBack(data, patient.Id);

            await _dataContext.SaveChangesAsync();
        }

        #endregion


        #region DeleteMoodRecord

        public async Task DeleteMoodRecord(int id)
        {
            var data = await _dataContext.Set<MoodRecord>()
                .FirstAsync(x => x.Id == id);

            _dataContext.Set<MoodRecord>()
                .Remove(data);

            await _dataContext.SaveChangesAsync();
        }

        #endregion
    }
}
