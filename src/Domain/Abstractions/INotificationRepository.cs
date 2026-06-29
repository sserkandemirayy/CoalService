using Domain.Entities;

namespace Domain.Abstractions;

public interface INotificationRepository
{
    Task<Notification?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<NotificationRecipient?> GetRecipientAsync(
        Guid notificationId,
        Guid userId,
        CancellationToken ct = default);

    Task AddAsync(Notification notification, CancellationToken ct = default);
    Task AddRecipientAsync(NotificationRecipient recipient, CancellationToken ct = default);

    Task<IReadOnlyList<NotificationRecipient>> GetUserNotificationsAsync(
        Guid userId,
        int page,
        int pageSize,
        CancellationToken ct = default);

    Task<int> GetUnreadCountAsync(Guid userId, CancellationToken ct = default);

    Task<IReadOnlyList<NotificationRecipient>> GetUnreadRecipientsAsync(
        Guid userId,
        CancellationToken ct = default);

    Task UpdateRecipientAsync(NotificationRecipient recipient, CancellationToken ct = default);

    IQueryable<Notification> Query();
    IQueryable<NotificationRecipient> RecipientQuery();
}