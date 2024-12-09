using CBT.Domain.Entities;

namespace CBT.Logic.Contracts
{
    public interface IMoodRecordService
    {
        Task<List<MoodRecord>> GetAllMoodRecords(string? userId = null);
        Task<int> AddMoodRecordFull(MoodRecord data, string? userId);
        Task<MoodRecord> GetMoodRecord(int id);
        Task EditMoodRecordFull(Action<MoodRecord, int> convertBack, int recordId, string? userId);
        Task DeleteMoodRecord(int id);
        Task<List<DateTime>> GetDateSuggestions(string userId);
        Task<List<TimeOnly>> GetTimeSuggestions(string userId); 
    }
}
