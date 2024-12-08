using CBT.Domain.Entities;

namespace CBT.Logic.Contracts
{
    public interface IAntiProcrastinationRecordService
    {
        Task<List<AntiProcrastinationRecord>> GetAllAntiProcrastinationRecords(string? userId = null);
        Task<int> AddAntiProcrastinationRecordFull(AntiProcrastinationRecord data, string? userId);
        Task<AntiProcrastinationRecord> GetAntiProcrastinationRecord(int id);
        Task EditAntiProcrastinationRecordFull(Action<AntiProcrastinationRecord, int> convertBack, int recordId, string? userId);
        Task DeleteAntiProcrastinationRecord(int id);
    }
}
