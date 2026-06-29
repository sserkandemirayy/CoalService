
using Application.Common.Notifications;
using Application.DTOs.Notifications;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;

namespace Infrastructure.Services;

public sealed class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly INotificationTemplateRepository _templateRepository;
    private readonly INotificationTargetResolver _targetResolver;
    private readonly INotificationSignalRNotifier _signalRNotifier;
    private readonly IUnitOfWork _unitOfWork;

    public NotificationService(
        INotificationRepository notificationRepository,
        INotificationTemplateRepository templateRepository,
        INotificationTargetResolver targetResolver,
        INotificationSignalRNotifier signalRNotifier,
        IUnitOfWork unitOfWork)
    {
        _notificationRepository = notificationRepository;
        _templateRepository = templateRepository;
        _targetResolver = targetResolver;
        _signalRNotifier = signalRNotifier;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> SendAsync(CreateNotificationRequest request, CancellationToken ct = default)
    {
        var userIds = await _targetResolver.ResolveAsync(request.TargetType, request.TargetIds, ct);

        return await CreateAndPushAsync(
            request.Title,
            request.Message,
            request.Type,
            request.Severity,
            userIds,
            request.SourceType,
            request.SourceId,
            request.ActionUrl,
            request.DataJson,
            request.TargetType == NotificationTargetType.Broadcast,
            request.ExpiresAt,
            ct);
    }

    public async Task<Guid> SendFromTemplateAsync(CreateTemplateNotificationRequest request, CancellationToken ct = default)
    {
        var template = await _templateRepository.GetByCodeAsync(request.TemplateCode, ct);
        if (template is null)
            throw new InvalidOperationException($"Notification template not found: {request.TemplateCode}");

        if (!template.IsActive)
            throw new InvalidOperationException($"Notification template is inactive: {request.TemplateCode}");

        var title = ApplyParameters(template.TitleTemplate, request.Parameters);
        var message = ApplyParameters(template.MessageTemplate, request.Parameters);
        var actionUrl = ApplyParameters(template.ActionUrlTemplate, request.Parameters);

        var userIds = await _targetResolver.ResolveAsync(request.TargetType, request.TargetIds, ct);

        return await CreateAndPushAsync(
            title,
            message,
            template.Type,
            template.Severity,
            userIds,
            request.SourceType,
            request.SourceId,
            actionUrl,
            request.DataJson,
            request.TargetType == NotificationTargetType.Broadcast,
            request.ExpiresAt,
            ct);
    }

    public async Task<Guid> SendToPermissionAsync(
        string permissionName,
        string title,
        string message,
        NotificationType type,
        NotificationSeverity severity,
        string? sourceType = null,
        Guid? sourceId = null,
        string? actionUrl = null,
        string? dataJson = null,
        CancellationToken ct = default)
    {
        var userIds = await _targetResolver.ResolveByPermissionAsync(permissionName, ct);

        return await CreateAndPushAsync(
            title,
            message,
            type,
            severity,
            userIds,
            sourceType,
            sourceId,
            actionUrl,
            dataJson,
            false,
            null,
            ct);
    }

    public async Task PushUnreadCountAsync(Guid userId, CancellationToken ct = default)
    {
        var count = await _notificationRepository.GetUnreadCountAsync(userId, ct);
        await _signalRNotifier.SendUnreadCountAsync(userId, count, ct);
    }

    private async Task<Guid> CreateAndPushAsync(
        string title,
        string message,
        NotificationType type,
        NotificationSeverity severity,
        IReadOnlyList<Guid> userIds,
        string? sourceType,
        Guid? sourceId,
        string? actionUrl,
        string? dataJson,
        bool isBroadcast,
        DateTime? expiresAt,
        CancellationToken ct)
    {
        var distinctUserIds = userIds
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToList();

        var notification = Notification.Create(
            title,
            message,
            type,
            severity,
            sourceType,
            sourceId,
            actionUrl,
            dataJson,
            isBroadcast,
            expiresAt);

        await _notificationRepository.AddAsync(notification, ct);

        foreach (var userId in distinctUserIds)
        {
            var recipient = NotificationRecipient.Create(notification.Id, userId);
            await _notificationRepository.AddRecipientAsync(recipient, ct);
        }

        await _unitOfWork.SaveChangesAsync(ct);

        var payload = new NotificationRealtimeDto(
            notification.Id,
            notification.Title,
            notification.Message,
            notification.Type.ToString(),
            notification.Severity.ToString(),
            notification.SourceType,
            notification.SourceId,
            notification.ActionUrl,
            notification.DataJson,
            notification.CreatedAt);

        foreach (var userId in distinctUserIds)
        {
            await _signalRNotifier.SendToUserAsync(userId, payload, ct);
            await PushUnreadCountAsync(userId, ct);
        }

        return notification.Id;
    }

    private static string ApplyParameters(string? template, Dictionary<string, string> parameters)
    {
        if (string.IsNullOrWhiteSpace(template))
            return string.Empty;

        var result = template;

        foreach (var parameter in parameters)
        {
            result = result.Replace("{" + parameter.Key + "}", parameter.Value ?? string.Empty);
        }

        return result;
    }
}