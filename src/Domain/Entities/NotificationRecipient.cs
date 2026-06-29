using Domain.Abstractions;

namespace Domain.Entities;

public class NotificationRecipient : BaseEntity
{
    protected NotificationRecipient() { }

    public Guid NotificationId { get; private set; }
    public Notification Notification { get; private set; } = default!;

    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!;

    public bool IsRead { get; private set; }
    public DateTime? ReadAt { get; private set; }

    public bool IsDelivered { get; private set; }
    public DateTime? DeliveredAt { get; private set; }

    public static NotificationRecipient Create(Guid notificationId, Guid userId)
    {
        if (notificationId == Guid.Empty)
            throw new ArgumentException("NotificationId is required.", nameof(notificationId));

        if (userId == Guid.Empty)
            throw new ArgumentException("UserId is required.", nameof(userId));

        return new NotificationRecipient
        {
            NotificationId = notificationId,
            UserId = userId,
            IsRead = false,
            IsDelivered = false
        };
    }

    public void MarkRead(DateTime? at = null)
    {
        if (IsRead)
            return;

        IsRead = true;
        ReadAt = at ?? DateTime.UtcNow;
    }

    public void MarkDelivered(DateTime? at = null)
    {
        if (IsDelivered)
            return;

        IsDelivered = true;
        DeliveredAt = at ?? DateTime.UtcNow;
    }
}