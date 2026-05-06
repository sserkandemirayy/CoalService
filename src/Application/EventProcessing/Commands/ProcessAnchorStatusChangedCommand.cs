using Application.Common.Models;
using Application.DTOs.EventProcessing;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.EventProcessing.Commands;

public sealed record ProcessAnchorStatusChangedCommand(AnchorStatusChangedPayloadDto Payload) : IRequest<Result<Guid>>;

public sealed class ProcessAnchorStatusChangedCommandHandler : IRequestHandler<ProcessAnchorStatusChangedCommand, Result<Guid>>
{
    private readonly IRawEventRepository _rawEventRepository;
    private readonly IAnchorRepository _anchorRepository;
    private readonly IAnchorStatusEventRepository _anchorStatusEventRepository;
    private readonly IAlarmRepository _alarmRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProcessAnchorStatusChangedCommandHandler(
        IRawEventRepository rawEventRepository,
        IAnchorRepository anchorRepository,
        IAnchorStatusEventRepository anchorStatusEventRepository,
        IAlarmRepository alarmRepository,
        IUnitOfWork unitOfWork)
    {
        _rawEventRepository = rawEventRepository;
        _anchorRepository = anchorRepository;
        _anchorStatusEventRepository = anchorStatusEventRepository;
        _alarmRepository = alarmRepository;
        _unitOfWork = unitOfWork;
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

        if (status is AnchorStatus.Offline or AnchorStatus.Error)
        {
            var alarmType = status == AnchorStatus.Offline
                ? AlarmType.AnchorOffline
                : AlarmType.AnchorError;

            var severity = status == AnchorStatus.Offline
                ? AlarmSeverity.Warning
                : AlarmSeverity.Critical;

            var alarm = Alarm.Create(
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

            await _alarmRepository.AddAsync(alarm, ct);
        }

        rawEvent.MarkProcessed();
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<Guid>.Success(statusEvent.Id);
    }
}