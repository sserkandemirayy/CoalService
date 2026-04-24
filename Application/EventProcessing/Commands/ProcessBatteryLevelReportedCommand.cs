using Application.Common.Models;
using Application.DTOs.EventProcessing;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;
using MediatR;

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

    public ProcessBatteryLevelReportedCommandHandler(
        IRawEventRepository rawEventRepository,
        ITagRepository tagRepository,
        IAnchorRepository anchorRepository,
        IBatteryEventRepository batteryEventRepository,
        IAlarmRepository alarmRepository,
        ITagAssignmentRepository tagAssignmentRepository,
        IUnitOfWork unitOfWork)
    {
        _rawEventRepository = rawEventRepository;
        _tagRepository = tagRepository;
        _anchorRepository = anchorRepository;
        _batteryEventRepository = batteryEventRepository;
        _alarmRepository = alarmRepository;
        _tagAssignmentRepository = tagAssignmentRepository;
        _unitOfWork = unitOfWork;
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
        }

        rawEvent.MarkProcessed();
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<Guid>.Success(batteryEvent.Id);
    }
}