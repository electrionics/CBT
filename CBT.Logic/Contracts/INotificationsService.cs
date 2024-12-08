using CBT.Domain.Entities;

namespace CBT.Logic.Contracts
{
    public interface INotificationsService
    {
        Task<Dictionary<int, int>> GetPsychologistNotifications(IEnumerable<int>? psychologistIds, CancellationToken stoppingToken = default);
        Task<Dictionary<int, int>> GetPatientNotifications(IEnumerable<int>? patientIds, CancellationToken stoppingToken = default);
        Task<List<Psychologist>> GetPsychologistsToNotify(IEnumerable<int> psychologistIds, CancellationToken stoppingToken = default);
        Task<List<Patient>> GetPatientsToNotify(IEnumerable<int> patientIds, CancellationToken stoppingToken = default);
    }
}
