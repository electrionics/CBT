using Microsoft.EntityFrameworkCore;

using CBT.Domain;
using CBT.Domain.Entities;

namespace CBT.Logic.Services
{
    public class AntiProcrastinationRecordService(CBTDataContext dataContext)
    {
        private readonly CBTDataContext _dataContext = dataContext;

        private const string DemoUserId = "DemoClient";

        #region GetAllAntiProcrastinationRecords

        public async Task<List<AntiProcrastinationRecord>> GetAllAntiProcrastinationRecords(string? userId = null)
        {
            var antiProcrastinationRecords = await _dataContext.Set<AntiProcrastinationRecord>()
                                .AsNoTracking()
                                .Where(x => x.Patient.UserId == (userId ?? DemoUserId))
                                .ToListAsync();

            return [.. antiProcrastinationRecords.OrderBy(CalculateOrderOfRecords)];
        }

        private static int CalculateOrderOfRecords(AntiProcrastinationRecord item)
        {
            var orderAddition = 0;

            if (item.PlanDate is null && item.DateTimeDone is null)
            {
                orderAddition += -2_000_000;
            }
            else if (item.DateTimeDone is null)
            {
                orderAddition += -1_000_000;
            }

            // item.PlanDate is null && item.DateTimeDone is not null - не должны встречаться такие записи

            return orderAddition + item.Id;
        }

        #endregion


        #region AddAntiProcrastinationRecordFull

        public async Task<int> AddAntiProcrastinationRecordFull(AntiProcrastinationRecord data, string? userId)
        {
            var patient = await _dataContext.Set<Patient>().FirstAsync(x => x.UserId == (userId ?? DemoUserId));

            _dataContext
                .Set<AntiProcrastinationRecord>()
                .Add(data);
            await _dataContext.SaveChangesAsync();

            return data.Id;
        }

        #endregion


        #region GetAntiProcrastinationRecord

        public async Task<AntiProcrastinationRecord> GetAntiProcrastinationRecord(int id)
        {
            return await _dataContext.Set<AntiProcrastinationRecord>()
                .AsNoTracking()
                .FirstAsync(x => x.Id == id);
        }

        #endregion


        #region EditAntiProcrastinationRecordFull

        public async Task EditAntiProcrastinationRecordFull(Action<AntiProcrastinationRecord, int> convertBack, int recordId, string? userId)
        {
            var patient = await _dataContext.Set<Patient>().FirstAsync(x => x.UserId == (userId ?? DemoUserId));

            var data = await _dataContext.Set<AntiProcrastinationRecord>()
                .FirstAsync(x => x.Id == recordId);

            convertBack(data, patient.Id);

            await _dataContext.SaveChangesAsync();
        }

        #endregion
    }
}
