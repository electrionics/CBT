using CBT.Domain.Entities;

namespace CBT.Logic.Contracts
{
    public interface IPeopleService
    {
        Task CreatePatient(string name, string userId);
        Task CreatePsychologist(string name, string userId);
        Task<Psychologist?> GetPsychologist(string userId);
        Task<Psychologist?> GetPsychologist(int psychologistId);
        Task<Patient?> GetPatient(string userId);
        Task<Patient?> GetPatient(int patientId);
        Task<PatientPsychologist?> GetExistingConnection(int patientId, int psychologistId);
        Task<bool> Connect(Patient? patient, Psychologist? psychologist, bool enable = true);
        Task<bool> DeleteConnection(Patient? patient, Psychologist? psychologist);
        Task<List<PatientPsychologist>> GetConnectionsFor(int? patientId, int? psychologistId);
    }
}
