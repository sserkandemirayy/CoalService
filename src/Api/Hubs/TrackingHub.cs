using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Api.Hubs;

[Authorize]
public sealed class TrackingHub : Hub
{
    public async Task JoinDashboard()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "dashboard");
    }

    public async Task LeaveDashboard()
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "dashboard");
    }

    public async Task JoinTagGroup(string tagId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"tag:{tagId}");
    }

    public async Task LeaveTagGroup(string tagId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"tag:{tagId}");
    }

    public async Task JoinAnchorGroup(string anchorId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"anchor:{anchorId}");
    }

    public async Task LeaveAnchorGroup(string anchorId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"anchor:{anchorId}");
    }
}