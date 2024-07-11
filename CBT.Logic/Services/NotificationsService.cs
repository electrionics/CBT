using CBT.Domain;
using CBT.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CBT.Logic.Services
{
    public class NotificationsService
    {
        private readonly CBTDataContextMARS dataContext;

        public NotificationsService(CBTDataContextMARS dataContext) 
        {
            this.dataContext = dataContext;
        }

        public async Task<Dictionary<int, int>> GetPsychologistNotifications(IEnumerable<int>? psychologistIds, CancellationToken stoppingToken = default)
        {
            var query = dataContext.Set<AutomaticThought>()
                .AsNoTracking()
                .Where(x => x.Sent && !x.PsychologistReviews.Any(y => y.SentBack))
                .SelectMany(x => x.Patient.Psychologists)
                .Where(x => psychologistIds == null || psychologistIds.Contains(x.PsychologistId))
                .GroupBy(x => x.PsychologistId)
                .Select(x => new { PsychologistId = x.Key, Notifications = x.Count() });

            return await query.ToDictionaryAsync(x => x.PsychologistId, x => x.Notifications, cancellationToken: stoppingToken); ;
        }

        public async Task<Dictionary<int, int>> GetPatientNotifications(IEnumerable<int>? patientIds, CancellationToken stoppingToken = default)
        {
            var result = await dataContext.Set<AutomaticThought>()
                        .AsNoTracking()
                        .Where(x => x.PsychologistReviews.Any(y => y.SentBack))
                        .Where(x => patientIds == null || patientIds.Contains(x.PatientId))
                        .GroupBy(x => x.PatientId)
                        .Select(x => new { PatientId = x.Key, Notifications = x.Count() })
                        .ToDictionaryAsync(x => x.PatientId, x => x.Notifications, cancellationToken: stoppingToken);

            return result;
        }

        public async Task<List<Psychologist>> GetPsychologistsToNotify(IEnumerable<int> psychologistIds, CancellationToken stoppingToken = default)
        {
            var psychologists = await dataContext.Set<Psychologist>()
                        .AsNoTracking()
                        .Where(x => psychologistIds.Contains(x.Id))
                        .ToListAsync(cancellationToken: stoppingToken);

            return psychologists;
        }

        public async Task<List<Patient>> GetPatientsToNotify(IEnumerable<int> patientIds, CancellationToken stoppingToken = default)
        {
            var patients = await dataContext.Set<Patient>()
                .AsNoTracking()
                .Where(x => patientIds.Contains(x.Id))
                .ToListAsync(cancellationToken: stoppingToken);

            return patients;
        }
    }
}
