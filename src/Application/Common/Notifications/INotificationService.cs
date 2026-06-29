using Application.DTOs.Notifications;
using Domain.Enums;

namespace Application.Common.Notifications;

public sealed record CreateNotificationRequest(
    string Title,
    string Message,
    NotificationType Type,
    NotificationSeverity Severity,
    NotificationTargetType TargetType,
    IReadOnlyList<Guid>? TargetIds,
    string? SourceType = null,
    Guid? SourceId = null,
    string? ActionUrl = null,
    string? DataJson = null,
    DateTime? ExpiresAt = null
);

public sealed record CreateTemplateNotificationRequest(
    string TemplateCode,
    NotificationTargetType TargetType,
    IReadOnlyList<Guid>? TargetIds,
    Dictionary<string, string> Parameters,
    string? SourceType = null,
    Guid? SourceId = null,
    string? DataJson = null,
    DateTime? ExpiresAt = null
);

public interface INotificationService
{
    Task<Guid> SendAsync(CreateNotificationRequest request, CancellationToken ct = default);
    Task<Guid> SendFromTemplateAsync(CreateTemplateNotificationRequest request, CancellationToken ct = default);

    Task<Guid> SendToPermissionAsync(
        string permissionName,
        string title,
        string message,
        NotificationType type,
        NotificationSeverity severity,
        string? sourceType = null,
        Guid? sourceId = null,
        string? actionUrl = null,
        string? dataJson = null,
        CancellationToken ct = default);

    Task PushUnreadCountAsync(Guid userId, CancellationToken ct = default);
}