using CBT.Web.Blazor.Data;
using CBT.Web.Blazor.Data.Entities;
using CBT.Web.Blazor.Hubs;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using System.Diagnostics;

namespace CBT.Web.Blazor.Background
{
    public class UserNotificationService : BackgroundService
    {
        private static readonly TimeSpan Period = TimeSpan.FromSeconds(3);
        private readonly ILogger<UserNotificationService> _logger;
        private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;

        public UserNotificationService(ILogger<UserNotificationService> logger, IHubContext<NotificationHub, INotificationClient> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(Period);

            while (!stoppingToken.IsCancellationRequested &&
                await timer.WaitForNextTickAsync(stoppingToken)) 
            {
                var time = DateTime.Now;

#pragma warning disable CA2254 // Template should be a static expression
                _logger.LogInformation($"Executing {nameof(UserNotificationService)} {time}");
#pragma warning restore CA2254 // Template should be a static expression

                using var dbContext = new CBTDataContext();
                var sw = Stopwatch.StartNew();

                var psychologistNotifications = await dbContext.Set<AutomaticThought>()
                    .AsNoTracking()
                    .Where(x => x.Sent && !x.SentBack)
                    .GroupBy(x => x.Patient.PsychologistId)
                    .Select(x => new { PsychologistId = x.Key, Notifications = x.Count() })
                    .ToDictionaryAsync(x => x.PsychologistId, x => x.Notifications, cancellationToken: stoppingToken);

                var patientNotifications = await dbContext.Set<AutomaticThought>()
                    .AsNoTracking()
                    .Where(x => x.SentBack)
                    .GroupBy(x => x.PatientId)
                    .Select(x => new { PatientId = x.Key, Notifications = x.Count() })
                    .ToDictionaryAsync(x => x.PatientId, x => x.Notifications, cancellationToken: stoppingToken);

                var psychologists = await dbContext.Set<Psychologist>()
                    .AsNoTracking()
                    .Where(x => psychologistNotifications.Keys.Contains(x.Id))
                    .ToListAsync(cancellationToken: stoppingToken);

                var patients = await dbContext.Set<Patient>()
                    .AsNoTracking()
                    .Where(x => patientNotifications.Keys.Contains(x.Id))
                    .ToListAsync(cancellationToken: stoppingToken);

                sw.Stop();

                var usersThatAlreadyReceivedNotifications = new HashSet<string>();

                foreach (var psychologist in psychologists)
                {
                    usersThatAlreadyReceivedNotifications.Add(psychologist.UserId);
                    var count = psychologistNotifications[psychologist.Id];
                    await _hubContext.Clients
                        .User(psychologist.UserId)
                        .ReceiveNotification(count.ToString());
                }

                foreach (var patient in patients)
                {
                    if (!usersThatAlreadyReceivedNotifications.Contains(patient.UserId))
                    {
                        var count = patientNotifications[patient.Id];
                        await _hubContext.Clients
                            .User(patient.UserId)
                            .ReceiveNotification(count.ToString());
                    }
                }

                //await _hubContext.Clients.All;
                //await _hubContext.Clients.User("ca97ca94-d587-4b3f-bc2b-fd5ba1d050f0").ReceiveNotification($"Server time is {time}");
            }
        }
    }
}
