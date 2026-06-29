using Application.DTOs.Notifications;

namespace Application.Common.Notifications;

public interface INotificationSignalRNotifier
{
    Task SendToUserAsync(Guid userId, NotificationRealtimeDto payload, CancellationToken ct = default);
    Task SendUnreadCountAsync(Guid userId, int count, CancellationToken ct = default);
}