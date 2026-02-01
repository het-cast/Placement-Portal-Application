using Microsoft.AspNetCore.SignalR;

namespace PlacementPortal.BLL.Service;

public class CustomUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.User?.FindFirst("id")?.Value!;
    }
}
