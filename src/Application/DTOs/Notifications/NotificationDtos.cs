using System.Text.Json;

namespace Application.DTOs.Notifications;

public sealed record NotificationDto(
    Guid Id,
    string Title,
    string Message,
    string Type,
    string Severity,
    string? SourceType,
    Guid? SourceId,
    string? ActionUrl,
    string? DataJson,
    bool IsRead,
    DateTime? ReadAt,
    bool IsDelivered,
    DateTime? DeliveredAt,
    DateTime CreatedAt
);

public sealed record NotificationUnreadCountDto(int Count);

public sealed record SendCustomNotificationDto(
    string Title,
    string Message,
    string Severity,
    string TargetType,
    List<Guid>? TargetIds,
    string? Type,
    string? SourceType,
    Guid? SourceId,
    string? ActionUrl,
    JsonElement? Data,
    DateTime? ExpiresAt
);

public sealed record SendFromTemplateDto(
    string TemplateCode,
    string TargetType,
    List<Guid>? TargetIds,
    Dictionary<string, string> Parameters,
    string? SourceType,
    Guid? SourceId,
    JsonElement? Data,
    DateTime? ExpiresAt
);

public sealed record NotificationTemplateDto(
    Guid Id,
    string Code,
    string Name,
    string TitleTemplate,
    string MessageTemplate,
    string? ActionUrlTemplate,
    string Type,
    string Severity,
    bool IsActive
);

public sealed record CreateNotificationTemplateDto(
    string Code,
    string Name,
    string TitleTemplate,
    string MessageTemplate,
    string Type,
    string Severity,
    string? ActionUrlTemplate
);

public sealed record UpdateNotificationTemplateDto(
    string Name,
    string TitleTemplate,
    string MessageTemplate,
    string Type,
    string Severity,
    string? ActionUrlTemplate
);

public sealed record NotificationRealtimeDto(
    Guid Id,
    string Title,
    string Message,
    string Type,
    string Severity,
    string? SourceType,
    Guid? SourceId,
    string? ActionUrl,
    string? DataJson,
    DateTime CreatedAt
);