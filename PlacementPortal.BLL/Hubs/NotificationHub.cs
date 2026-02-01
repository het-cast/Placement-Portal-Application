using Microsoft.AspNetCore.SignalR;

namespace PlacementPortal.BLL.Hubs;

public class NotificationHub : Hub
{
    private readonly IServiceProvider _serviceProvider;

    public NotificationHub(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    public async Task SendNotification(string userId, string message)
    {
        await Clients.User(userId).SendAsync("ReceiveNotification", message);
    }
}
