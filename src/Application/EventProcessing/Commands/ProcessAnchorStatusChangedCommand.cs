using Application.Common.Models;
using Application.DTOs.EventProcessing;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Application.Common.Realtime;
using Application.Common.Notifications;

namespace Application.EventProcessing.Commands;

public sealed record ProcessAnchorStatusChangedCommand(AnchorStatusChangedPayloadDto Payload) : IRequest<Result<Guid>>;

public sealed class ProcessAnchorStatusChangedCommandHandler : IRequestHandler<ProcessAnchorStatusChangedCommand, Result<Guid>>
{
    private readonly IRawEventRepository _rawEventRepository;
    private readonly IAnchorRepository _anchorRepository;
    private readonly IAnchorStatusEventRepository _anchorStatusEventRepository;
    private readonly IAlarmRepository _alarmRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRealtimeNotifier _realtimeNotifier;
    private readonly INotificationService _notificationService;


    public ProcessAnchorStatusChangedCommandHandler(
        IRawEventRepository rawEventRepository,
        IAnchorRepository anchorRepository,
        IAnchorStatusEventRepository anchorStatusEventRepository,
        IAlarmRepository alarmRepository,
        IUnitOfWork unitOfWork,
        IRealtimeNotifier realtimeNotifier,
        INotificationService notificationService)
    {
        _rawEventRepository = rawEventRepository;
        _anchorRepository = anchorRepository;
        _anchorStatusEventRepository = anchorStatusEventRepository;
        _alarmRepository = alarmRepository;
        _unitOfWork = unitOfWork;
        _realtimeNotifier = realtimeNotifier;
        _notificationService = notificationService;
    }

    public async Task<Result<Guid>> Handle(ProcessAnchorStatusChangedCommand request, CancellationToken ct)
    {
        if (await _rawEventRepository.ExistsByExternalEventIdAsync(request.Payload.Id, ct))
            return Result<Guid>.Failure("Duplicate raw event.");

        var eventAt = EventProcessingHelper.FromUnixMilliseconds(request.Payload.Timestamp);

        var rawEvent = RawEvent.Create(
            request.Payload.Id,
            request.Payload.Type,
            eventAt,
            EventProcessingHelper.Serialize(request.Payload),
            null,
            request.Payload.AnchorId);

        await _rawEventRepository.AddAsync(rawEvent, ct);

        var anchor = await _anchorRepository.GetByExternalIdAsync(request.Payload.AnchorId, ct);
        if (anchor is null)
        {
            rawEvent.MarkFailed("Anchor not found.");
            await _rawEventRepository.UpdateAsync(rawEvent, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            return Result<Guid>.Failure("Anchor not found.");
        }

        var status = EventProcessingHelper.ParseAnchorStatus(request.Payload.Status);
        var previousStatus = EventProcessingHelper.ParseAnchorStatus(request.Payload.PreviousStatus);

        var statusEvent = AnchorStatusEvent.Create(
            rawEvent.Id,
            anchor.Id,
            eventAt,
            status,
            previousStatus,
            request.Payload.Reason);

        await _anchorStatusEventRepository.AddAsync(statusEvent, ct);

        anchor.ChangeStatus(status, eventAt);
        await _anchorRepository.UpdateAsync(anchor, ct);

        Alarm? createdAlarm = null;

        if (status is AnchorStatus.Offline or AnchorStatus.Error)
        {

            var alarmType = status == AnchorStatus.Offline
                ? AlarmType.AnchorOffline
                : AlarmType.AnchorError;

            var severity = status == AnchorStatus.Offline
                ? AlarmSeverity.Warning
                : AlarmSeverity.Critical;

            createdAlarm = Alarm.Create(
                alarmType,
                severity,
                $"Anchor status changed: {status}",
                eventAt,
                rawEvent.Id,
                null,
                null,
                anchor.Id,
                null,
                request.Payload.Reason,
                EventProcessingHelper.Serialize(request.Payload));

            await _alarmRepository.AddAsync(createdAlarm, ct);

        }

        rawEvent.MarkProcessed();
        await _unitOfWork.SaveChangesAsync(ct);

        await _realtimeNotifier.AnchorStatusChangedAsync(
            new AnchorStatusChangedRealtimeDto(
                anchor.Id,
                anchor.ExternalId,
                anchor.Code,
                status.ToString(),
                previousStatus.ToString(),
                request.Payload.Reason,
                eventAt),
            ct);

        if (createdAlarm is not null)
        {
            await _realtimeNotifier.AlarmRaisedAsync(
                new AlarmRaisedRealtimeDto(
                    createdAlarm.Id,
                    createdAlarm.AlarmType.ToString(),
                    createdAlarm.Severity.ToString(),
                    createdAlarm.Status.ToString(),
                    createdAlarm.Title,
                    null,
                    null,
                    null,
                    null,
                    anchor.Id,
                    anchor.ExternalId,
                    null,
                    createdAlarm.StartedAt),
                ct);

            await _notificationService.SendToPermissionAsync(
                    "view_devices",
                    $"Anchor Durumu: {status}",
                    $"{anchor.Code} anchor durumu {status} olarak değişti.",
                    NotificationType.Anchor,
                    status == AnchorStatus.Error ? NotificationSeverity.Critical : NotificationSeverity.Warning,
                    "Alarm",
                    createdAlarm.Id,
                    $"/anchors/{anchor.Id}",
                    EventProcessingHelper.Serialize(new
                    {
                        AlarmId = createdAlarm.Id,
                        AnchorId = anchor.Id,
                        AnchorCode = anchor.Code,
                        Status = status.ToString(),
                        PreviousStatus = previousStatus.ToString(),
                        Reason = request.Payload.Reason
                    }),
                    ct);
        }

        return Result<Guid>.Success(statusEvent.Id);
    }
}