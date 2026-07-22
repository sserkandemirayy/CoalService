using Application.Common.Mappings;
using Application.Common.Models;
using Application.Common.Realtime;
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
    private readonly IRealtimeNotifier _realtimeNotifier;
    private readonly ITagAssignmentRepository _tagAssignmentRepository;

    public ProcessProximityAlertRaisedCommandHandler(
        IRawEventRepository rawEventRepository,
        ITagRepository tagRepository,
        IProximityEventRepository proximityEventRepository,
        IAlarmRepository alarmRepository,
        ITagAssignmentRepository tagAssignmentRepository,
        IUnitOfWork unitOfWork,
        IRealtimeNotifier realtimeNotifier)
    {
        _rawEventRepository = rawEventRepository;
        _tagRepository = tagRepository;
        _proximityEventRepository = proximityEventRepository;
        _alarmRepository = alarmRepository;
        _tagAssignmentRepository = tagAssignmentRepository;
        _unitOfWork = unitOfWork;
        _realtimeNotifier = realtimeNotifier;
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

        var activeAssignment = await _tagAssignmentRepository.GetActiveByTagIdAsync(tag.Id, ct);
        var peerAssignment = await _tagAssignmentRepository.GetActiveByTagIdAsync(peerTag.Id, ct);
        var assignedUser = activeAssignment?.User;
        var peerUser = peerAssignment?.User;

        await _realtimeNotifier.AlarmRaisedAsync(
                new AlarmRaisedRealtimeDto(
                    alarm.Id,
                    alarm.AlarmType.ToString(),
                    alarm.Severity.ToString(),
                    alarm.Status.ToString(),
                    alarm.Title,
                    tag.Id,
                    tag.ExternalId,
                    peerTag.Id,
                    peerTag.ExternalId,
                    null,
                    null,
                    null,
                    alarm.StartedAt),
                ct);

        //await _realtimeNotifier.TagStatusChangedAsync(
        //    new TagStatusChangedRealtimeDto(
        //        tag.Id,
        //        tag.ExternalId,
        //        tag.Code,
        //        tag.Status.ToString(),
        //        eventAt),
        //    ct);

        await _realtimeNotifier.TagStatusChangedAsync(
            new TagStatusChangedRealtimeDto(
                tag.Id,
                tag.ExternalId,
                tag.Code,
                tag.TagType.ToString(),
                assignedUser?.Id,
                assignedUser?.GetFullName(),
                assignedUser?.Identifier,
                tag.Status.ToString(),
                eventAt),
            ct);

        //await _realtimeNotifier.TagStatusChangedAsync(
        //    new TagStatusChangedRealtimeDto(
        //        peerTag.Id,
        //        peerTag.ExternalId,
        //        peerTag.Code,
        //        peerTag.Status.ToString(),
        //        eventAt),
        //    ct);

        await _realtimeNotifier.TagStatusChangedAsync(
            new TagStatusChangedRealtimeDto(
                peerTag.Id,
                peerTag.ExternalId,
                peerTag.Code,
                peerTag.TagType.ToString(),
                peerUser?.Id,
                peerUser?.GetFullName(),
                peerUser?.Identifier,
                peerTag.Status.ToString(),
                eventAt),
            ct);

        return Result<Guid>.Success(proximityEvent.Id);
    }
}