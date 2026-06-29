using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities;

public class Notification : BaseEntity
{
    protected Notification() { }

    public string Title { get; private set; } = default!;
    public string Message { get; private set; } = default!;

    public NotificationType Type { get; private set; }
    public NotificationSeverity Severity { get; private set; }

    public string? SourceType { get; private set; }
    public Guid? SourceId { get; private set; }

    public string? ActionUrl { get; private set; }
    public string? DataJson { get; private set; }

    public bool IsBroadcast { get; private set; }
    public DateTime? ExpiresAt { get; private set; }

    public ICollection<NotificationRecipient> Recipients { get; private set; } = new List<NotificationRecipient>();

    public static Notification Create(
        string title,
        string message,
        NotificationType type,
        NotificationSeverity severity,
        string? sourceType = null,
        Guid? sourceId = null,
        string? actionUrl = null,
        string? dataJson = null,
        bool isBroadcast = false,
        DateTime? expiresAt = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));

        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message is required.", nameof(message));

        return new Notification
        {
            Title = title.Trim(),
            Message = message.Trim(),
            Type = type,
            Severity = severity,
            SourceType = string.IsNullOrWhiteSpace(sourceType) ? null : sourceType.Trim(),
            SourceId = sourceId,
            ActionUrl = string.IsNullOrWhiteSpace(actionUrl) ? null : actionUrl.Trim(),
            DataJson = string.IsNullOrWhiteSpace(dataJson) ? null : dataJson,
            IsBroadcast = isBroadcast,
            ExpiresAt = expiresAt
        };
    }
}