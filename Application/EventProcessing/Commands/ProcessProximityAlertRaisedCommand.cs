using Application.Common.Models;
using Application.DTOs.EventProcessing;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.EventProcessing.Commands;

public sealed record ProcessProximityAlertRaisedCommand(ProximityAlertRaisedPayloadDto Payload) : IRequest<Result<Guid>>;

public sealed class ProcessProximityAlertRaisedCommandHandler : IRequestHandler<ProcessProximityAlertRaisedCommand, Result<Guid>>
{
    private readonly IRawEventRepository _rawEventRepository;
    private readonly ITagRepository _tagRepository;
    private readonly IProximityEventRepository _proximityEventRepository;
    private readonly IAlarmRepository _alarmRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProcessProximityAlertRaisedCommandHandler(
        IRawEventRepository rawEventRepository,
        ITagRepository tagRepository,
        IProximityEventRepository proximityEventRepository,
        IAlarmRepository alarmRepository,
        IUnitOfWork unitOfWork)
    {
        _rawEventRepository = rawEventRepository;
        _tagRepository = tagRepository;
        _proximityEventRepository = proximityEventRepository;
        _alarmRepository = alarmRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(ProcessProximityAlertRaisedCommand request, CancellationToken ct)
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
            null);

        await _rawEventRepository.AddAsync(rawEvent, ct);

        var tag = await _tagRepository.GetByExternalIdAsync(request.Payload.TagId, ct);
        if (tag is null)
        {
            rawEvent.MarkFailed("Primary tag not found.");
            await _unitOfWork.SaveChangesAsync(ct);
            return Result<Guid>.Failure("Primary tag not found.");
        }

        var peerTag = await _tagRepository.GetByExternalIdAsync(request.Payload.PeerTagId, ct);
        if (peerTag is null)
        {
            rawEvent.MarkFailed("Peer tag not found.");
            await _unitOfWork.SaveChangesAsync(ct);
            return Result<Guid>.Failure("Peer tag not found.");
        }

        var severity = EventProcessingHelper.ParseProximitySeverity(request.Payload.Severity);

        var proximityEvent = ProximityEvent.Create(
            rawEvent.Id,
            tag.Id,
            peerTag.Id,
            eventAt,
            request.Payload.Distance,
            request.Payload.Threshold,
            severity);

        await _proximityEventRepository.AddAsync(proximityEvent, ct);

        var alarmSeverity = severity == ProximitySeverity.Critical
            ? AlarmSeverity.Critical
            : AlarmSeverity.Warning;

        var alarm = Alarm.Create(
            AlarmType.ProximityAlert,
            alarmSeverity,
            "Proximity alert",
            eventAt,
            rawEvent.Id,
            tag.Id,
            peerTag.Id,
            null,
            null,
            "Proximity threshold exceeded.",
            EventProcessingHelper.Serialize(request.Payload));

        await _alarmRepository.AddAsync(alarm, ct);

        tag.MarkSeen(eventAt);
        peerTag.MarkSeen(eventAt);

        await _tagRepository.UpdateAsync(tag, ct);
        await _tagRepository.UpdateAsync(peerTag, ct);

        rawEvent.MarkProcessed();
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<Guid>.Success(proximityEvent.Id);
    }
}