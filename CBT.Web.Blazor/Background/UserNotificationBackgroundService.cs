using System.Diagnostics;

using Microsoft.AspNetCore.SignalR;

using CBT.Web.Blazor.Data;
using CBT.Web.Blazor.Hubs;
using CBT.Web.Blazor.Services;

namespace CBT.Web.Blazor.Background
{
    public class UserNotificationBackgroundService : BackgroundService
    {
        private static readonly TimeSpan Period = TimeSpan.FromSeconds(30);
        private readonly ILogger<UserNotificationBackgroundService> _logger;
        private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;
        private readonly DatabaseConfig databaseConfig;

        public UserNotificationBackgroundService(ILogger<UserNotificationBackgroundService> logger, IHubContext<NotificationHub, INotificationClient> hubContext, DatabaseConfig databaseConfig)
        {
            _logger = logger;
            _hubContext = hubContext;
            this.databaseConfig = databaseConfig;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(Period);

            while (!stoppingToken.IsCancellationRequested &&
                await timer.WaitForNextTickAsync(stoppingToken)) 
            {
                try
                {
                    var time = DateTime.Now;

#pragma warning disable CA2254 // Template should be a static expression
                    _logger.LogInformation($"Executing {nameof(UserNotificationBackgroundService)} {time}");
#pragma warning restore CA2254 // Template should be a static expression

                    using var dbContext = new CBTDataContext(databaseConfig.SingleConnectionString);
                    var notificationService = new NotificationsService(dbContext);

                    var sw = Stopwatch.StartNew();

                    var psychologistNotifications = await notificationService.GetPsychologistNotifications(null, stoppingToken);
                    var patientNotifications = await notificationService.GetPatientNotifications(null, stoppingToken);

                    var psychologistIds = psychologistNotifications.Keys.ToHashSet();
                    var psychologists = await notificationService.GetPsychologistsToNotify(psychologistIds, stoppingToken);

                    var patientIds = psychologistNotifications.Keys.ToHashSet();
                    var patients = await notificationService.GetPatientsToNotify(patientIds, stoppingToken);

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
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Notifications error.");
                }

                //await _hubContext.Clients.All;
                //await _hubContext.Clients.User("ca97ca94-d587-4b3f-bc2b-fd5ba1d050f0").ReceiveNotification($"Server time is {time}");
            }
        }
    }
}
