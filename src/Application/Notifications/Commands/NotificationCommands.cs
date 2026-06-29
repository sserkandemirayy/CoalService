using System.Text.Json;
using Application.Common.Models;
using Application.Common.Notifications;
using Application.DTOs.Notifications;
using Domain.Abstractions;
using Domain.Enums;
using MediatR;

namespace Application.Notifications.Commands;

public sealed record MarkNotificationReadCommand(Guid UserId, Guid NotificationId)
    : IRequest<Result<Unit>>;

public sealed class MarkNotificationReadCommandHandler
    : IRequestHandler<MarkNotificationReadCommand, Result<Unit>>
{
    private readonly INotificationRepository _repository;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;

    public MarkNotificationReadCommandHandler(
        INotificationRepository repository,
        INotificationService notificationService,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Unit>> Handle(MarkNotificationReadCommand request, CancellationToken ct)
    {
        var recipient = await _repository.GetRecipientAsync(request.NotificationId, request.UserId, ct);
        if (recipient is null)
            return Result<Unit>.Failure("Notification not found.");

        recipient.MarkRead();
        await _repository.UpdateRecipientAsync(recipient, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        await _notificationService.PushUnreadCountAsync(request.UserId, ct);

        return Result<Unit>.Success(Unit.Value);
    }
}

public sealed record MarkAllNotificationsReadCommand(Guid UserId)
    : IRequest<Result<Unit>>;

public sealed class MarkAllNotificationsReadCommandHandler
    : IRequestHandler<MarkAllNotificationsReadCommand, Result<Unit>>
{
    private readonly INotificationRepository _repository;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;

    public MarkAllNotificationsReadCommandHandler(
        INotificationRepository repository,
        INotificationService notificationService,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Unit>> Handle(MarkAllNotificationsReadCommand request, CancellationToken ct)
    {
        var unread = await _repository.GetUnreadRecipientsAsync(request.UserId, ct);

        foreach (var item in unread)
        {
            item.MarkRead();
            await _repository.UpdateRecipientAsync(item, ct);
        }

        await _unitOfWork.SaveChangesAsync(ct);
        await _notificationService.PushUnreadCountAsync(request.UserId, ct);

        return Result<Unit>.Success(Unit.Value);
    }
}

public sealed record DeleteNotificationCommand(Guid UserId, Guid NotificationId)
    : IRequest<Result<Unit>>;

public sealed class DeleteNotificationCommandHandler
    : IRequestHandler<DeleteNotificationCommand, Result<Unit>>
{
    private readonly INotificationRepository _repository;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteNotificationCommandHandler(
        INotificationRepository repository,
        INotificationService notificationService,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Unit>> Handle(DeleteNotificationCommand request, CancellationToken ct)
    {
        var recipient = await _repository.GetRecipientAsync(request.NotificationId, request.UserId, ct);
        if (recipient is null)
            return Result<Unit>.Failure("Notification not found.");

        recipient.SoftDelete(request.UserId);
        await _repository.UpdateRecipientAsync(recipient, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        await _notificationService.PushUnreadCountAsync(request.UserId, ct);

        return Result<Unit>.Success(Unit.Value);
    }
}

public sealed record SendCustomNotificationCommand(
    Guid PerformedByUserId,
    SendCustomNotificationDto Dto
) : IRequest<Result<Guid>>;

public sealed class SendCustomNotificationCommandHandler
    : IRequestHandler<SendCustomNotificationCommand, Result<Guid>>
{
    private readonly INotificationService _notificationService;

    public SendCustomNotificationCommandHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task<Result<Guid>> Handle(SendCustomNotificationCommand request, CancellationToken ct)
    {
        if (!Enum.TryParse<NotificationSeverity>(request.Dto.Severity, true, out var severity))
            return Result<Guid>.Failure("Invalid notification severity.");

        var type = NotificationType.Custom;
        if (!string.IsNullOrWhiteSpace(request.Dto.Type) &&
            !Enum.TryParse<NotificationType>(request.Dto.Type, true, out type))
            return Result<Guid>.Failure("Invalid notification type.");

        if (!Enum.TryParse<NotificationTargetType>(request.Dto.TargetType, true, out var targetType))
            return Result<Guid>.Failure("Invalid notification target type.");

        var dataJson = request.Dto.Data.HasValue
            ? JsonSerializer.Serialize(request.Dto.Data.Value)
            : null;

        var id = await _notificationService.SendAsync(
            new CreateNotificationRequest(
                request.Dto.Title,
                request.Dto.Message,
                type,
                severity,
                targetType,
                request.Dto.TargetIds,
                request.Dto.SourceType,
                request.Dto.SourceId,
                request.Dto.ActionUrl,
                dataJson,
                request.Dto.ExpiresAt),
            ct);

        return Result<Guid>.Success(id);
    }
}

public sealed record SendNotificationFromTemplateCommand(
    Guid PerformedByUserId,
    SendFromTemplateDto Dto
) : IRequest<Result<Guid>>;

public sealed class SendNotificationFromTemplateCommandHandler
    : IRequestHandler<SendNotificationFromTemplateCommand, Result<Guid>>
{
    private readonly INotificationService _notificationService;

    public SendNotificationFromTemplateCommandHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task<Result<Guid>> Handle(SendNotificationFromTemplateCommand request, CancellationToken ct)
    {
        if (!Enum.TryParse<NotificationTargetType>(request.Dto.TargetType, true, out var targetType))
            return Result<Guid>.Failure("Invalid notification target type.");

        var dataJson = request.Dto.Data.HasValue
            ? JsonSerializer.Serialize(request.Dto.Data.Value)
            : null;

        var id = await _notificationService.SendFromTemplateAsync(
            new CreateTemplateNotificationRequest(
                request.Dto.TemplateCode,
                targetType,
                request.Dto.TargetIds,
                request.Dto.Parameters,
                request.Dto.SourceType,
                request.Dto.SourceId,
                dataJson,
                request.Dto.ExpiresAt),
            ct);

        return Result<Guid>.Success(id);
    }
}