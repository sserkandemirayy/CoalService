using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Api.Hubs;

[Authorize]
public sealed class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();

        if (userId.HasValue)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user:{userId.Value}");
        }

        await base.OnConnectedAsync();
    }

    public async Task JoinNotifications()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "notifications");
    }

    public async Task LeaveNotifications()
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "notifications");
    }

    private Guid? GetUserId()
    {
        var value =
            Context.User?.FindFirstValue(ClaimTypes.NameIdentifier) ??
            Context.User?.FindFirstValue("sub") ??
            Context.User?.FindFirstValue("userId");

        return Guid.TryParse(value, out var id) ? id : null;
    }
}