using Api.Hubs;
using Application.Common.Notifications;
using Application.DTOs.Notifications;
using Microsoft.AspNetCore.SignalR;

namespace Api.Services;

public sealed class NotificationSignalRNotifier : INotificationSignalRNotifier
{
    private readonly IHubContext<NotificationHub> _hub;

    public NotificationSignalRNotifier(IHubContext<NotificationHub> hub)
    {
        _hub = hub;
    }

    public async Task SendToUserAsync(Guid userId, NotificationRealtimeDto payload, CancellationToken ct = default)
    {
        await _hub.Clients.Group($"user:{userId}")
            .SendAsync("NotificationReceived", payload, ct);
    }

    public async Task SendUnreadCountAsync(Guid userId, int count, CancellationToken ct = default)
    {
        await _hub.Clients.Group($"user:{userId}")
            .SendAsync("NotificationUnreadCountChanged", count, ct);
    }
}