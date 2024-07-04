using Microsoft.AspNetCore.SignalR;

namespace CBT.Web.Blazor.Hubs
{
    public class NotificationHub:Hub<INotificationClient>
    {
        public async override Task OnConnectedAsync()
        {
            await Clients.Client(Context.ConnectionId).ReceiveNotification("0");
        }
    }

    public interface INotificationClient
    {
        Task ReceiveNotification(string message);
    }
}