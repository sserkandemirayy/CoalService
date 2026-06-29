using Application.Common.Models;
using Application.DTOs.EventProcessing;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Application.Common.Realtime;
using Application.Common.Notifications;

namespace Application.EventProcessing.Commands;

public sealed record ProcessBatteryLevelReportedCommand(BatteryLevelReportedPayloadDto Payload) : IRequest<Result<Guid>>;

public sealed class ProcessBatteryLevelReportedCommandHandler : IRequestHandler<ProcessBatteryLevelReportedCommand, Result<Guid>>
{
    private readonly IRawEventRepository _rawEventRepository;
    private readonly ITagRepository _tagRepository;
    private readonly IAnchorRepository _anchorRepository;
    private readonly IBatteryEventRepository _batteryEventRepository;
    private readonly IAlarmRepository _alarmRepository;
    private readonly ITagAssignmentRepository _tagAssignmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRealtimeNotifier _realtimeNotifier;
    private readonly INotificationService _notificationService;

    public ProcessBatteryLevelReportedCommandHandler(
        IRawEventRepository rawEventRepository,
        ITagRepository tagRepository,
        IAnchorRepository anchorRepository,
        IBatteryEventRepository batteryEventRepository,
        IAlarmRepository alarmRepository,
        ITagAssignmentRepository tagAssignmentRepository,
        IUnitOfWork unitOfWork,
        IRealtimeNotifier realtimeNotifier,
        INotificationService notificationService)
    {
        _rawEventRepository = rawEventRepository;
        _tagRepository = tagRepository;
        _anchorRepository = anchorRepository;
        _batteryEventRepository = batteryEventRepository;
        _alarmRepository = alarmRepository;
        _tagAssignmentRepository = tagAssignmentRepository;
        _unitOfWork = unitOfWork;
        _realtimeNotifier = realtimeNotifier;
        _notificationService = notificationService;
    }

    public async Task<Result<Guid>> Handle(ProcessBatteryLevelReportedCommand request, CancellationToken ct)
    {
        if (await _rawEventRepository.ExistsByExternalEventIdAsync(request.Payload.Id, ct))
            return Result<Guid>.Failure("Duplicate raw event.");

        var eventAt = EventProcessingHelper.FromUnixMilliseconds(request.Payload.Timestamp);

        var rawEvent = RawEvent.Create(
            request.Payload.Id,
            request.Payload.Type,
            eventAt,
            EventProcessingHelper.Serialize(request.Payload),
            request.Payload.TagId,
            request.Payload.AnchorId);

        await _rawEventRepository.AddAsync(rawEvent, ct);

        var tag = await _tagRepository.GetByExternalIdAsync(request.Payload.TagId, ct);
        if (tag is null)
        {
            rawEvent.MarkFailed("Tag not found.");
            await _unitOfWork.SaveChangesAsync(ct);
            return Result<Guid>.Failure("Tag not found.");
        }

        var anchor = await _anchorRepository.GetByExternalIdAsync(request.Payload.AnchorId, ct);
        if (anchor is null)
        {
            rawEvent.MarkFailed("Anchor not found.");
            await _unitOfWork.SaveChangesAsync(ct);
            return Result<Guid>.Failure("Anchor not found.");
        }

        var batteryEvent = BatteryEvent.Create(
            rawEvent.Id,
            tag.Id,
            anchor.Id,
            eventAt,
            request.Payload.BatteryLevel);

        await _batteryEventRepository.AddAsync(batteryEvent, ct);

        tag.UpdateBattery(request.Payload.BatteryLevel, eventAt);
        await _tagRepository.UpdateAsync(tag, ct);

        if (request.Payload.BatteryLevel <= 20)
        {
            var assignment = await _tagAssignmentRepository.GetActiveByTagIdAsync(tag.Id, ct);

            var severity = request.Payload.BatteryLevel <= 10
                ? AlarmSeverity.Critical
                : AlarmSeverity.Warning;

            var alarm = Alarm.Create(
                AlarmType.LowBattery,
                severity,
                "Low battery",
                eventAt,
                rawEvent.Id,
                tag.Id,
                null,
                anchor.Id,
                assignment?.UserId,
                "Battery level reported below threshold.",
                EventProcessingHelper.Serialize(request.Payload));

            await _alarmRepository.AddAsync(alarm, ct);

            await _realtimeNotifier.AlarmRaisedAsync(
                new AlarmRaisedRealtimeDto(
                    alarm.Id,
                    alarm.AlarmType.ToString(),
                    alarm.Severity.ToString(),
                    alarm.Status.ToString(),
                    alarm.Title,
                    tag.Id,
                    tag.ExternalId,
                    null,
                    null,
                    anchor.Id,
                    anchor.ExternalId,
                    assignment?.UserId,
                    alarm.StartedAt),
                ct);

            await _notificationService.SendToPermissionAsync(
                "view_devices",
                "Düşük Batarya",
                $"{tag.Code} batarya seviyesi %{request.Payload.BatteryLevel}.",
                NotificationType.Battery,
                severity == AlarmSeverity.Critical ? NotificationSeverity.Critical : NotificationSeverity.Warning,
                "Alarm",
                alarm.Id,
                $"/tags/{tag.Id}",
                EventProcessingHelper.Serialize(new
                {
                    AlarmId = alarm.Id,
                    TagId = tag.Id,
                    TagCode = tag.Code,
                    BatteryLevel = request.Payload.BatteryLevel,
                    AnchorId = anchor.Id,
                    AnchorCode = anchor.Code
                }),
                ct);
        }

        rawEvent.MarkProcessed();
        await _unitOfWork.SaveChangesAsync(ct);

        await _realtimeNotifier.BatteryUpdatedAsync(
            new BatteryUpdatedRealtimeDto(
                tag.Id,
                tag.ExternalId,
                tag.Code,
                anchor.Id,
                anchor.ExternalId,
                request.Payload.BatteryLevel,
                eventAt),
            ct);

        return Result<Guid>.Success(batteryEvent.Id);
    }
}