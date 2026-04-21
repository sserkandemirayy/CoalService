using Application.Common.Models;
using Application.DTOs.EventProcessing;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.EventProcessing.Commands;

public sealed record ProcessAnchorConfigReportedCommand(AnchorConfigReportedPayloadDto Payload) : IRequest<Result<Guid>>;

public sealed class ProcessAnchorConfigReportedCommandHandler : IRequestHandler<ProcessAnchorConfigReportedCommand, Result<Guid>>
{
    private readonly IRawEventRepository _rawEvents;
    private readonly IAnchorRepository _anchors;
    private readonly IAnchorConfigEventRepository _configEvents;
    private readonly IAnchorConfigSnapshotRepository _snapshots;
    private readonly IUnitOfWork _uow;

    public ProcessAnchorConfigReportedCommandHandler(
        IRawEventRepository rawEvents,
        IAnchorRepository anchors,
        IAnchorConfigEventRepository configEvents,
        IAnchorConfigSnapshotRepository snapshots,
        IUnitOfWork uow)
    {
        _rawEvents = rawEvents;
        _anchors = anchors;
        _configEvents = configEvents;
        _snapshots = snapshots;
        _uow = uow;
    }

    public async Task<Result<Guid>> Handle(ProcessAnchorConfigReportedCommand request, CancellationToken ct)
    {
        if (await _rawEvents.ExistsByExternalEventIdAsync(request.Payload.Id, ct))
            return Result<Guid>.Failure("Duplicate raw event.");

        var anchor = await _anchors.GetByExternalIdAsync(request.Payload.AnchorId, ct);
        if (anchor is null)
            return Result<Guid>.Failure("Anchor not found.");

        var eventAt = EventProcessingHelper.FromUnixMilliseconds(request.Payload.Timestamp);

        var positionJson = EventProcessingHelper.Serialize(request.Payload.Position);
        var networkJson = EventProcessingHelper.Serialize(request.Payload.Network);
        var uwbJson = EventProcessingHelper.Serialize(request.Payload.Uwb);
        var bleJson = EventProcessingHelper.Serialize(request.Payload.Ble);

        var raw = RawEvent.Create(
            request.Payload.Id,
            request.Payload.Type,
            eventAt,
            EventProcessingHelper.Serialize(request.Payload),
            null,
            request.Payload.AnchorId);

        await _rawEvents.AddAsync(raw, ct);

        var configEvent = AnchorConfigEvent.Create(
            raw.Id,
            anchor.Id,
            eventAt,
            request.Payload.FirmwareVersion,
            positionJson,
            networkJson,
            uwbJson,
            bleJson,
            request.Payload.HeartbeatInterval,
            request.Payload.ReportInterval);

        await _configEvents.AddAsync(configEvent, ct);

        var snapshot = await _snapshots.GetByAnchorIdAsync(anchor.Id, ct);
        if (snapshot is null)
        {
            snapshot = AnchorConfigSnapshot.Create(
                anchor.Id,
                raw.Id,
                eventAt,
                request.Payload.FirmwareVersion,
                positionJson,
                networkJson,
                uwbJson,
                bleJson,
                request.Payload.HeartbeatInterval,
                request.Payload.ReportInterval);

            await _snapshots.AddAsync(snapshot, ct);
        }
        else
        {
            snapshot.UpdateSnapshot(
                raw.Id,
                eventAt,
                request.Payload.FirmwareVersion,
                positionJson,
                networkJson,
                uwbJson,
                bleJson,
                request.Payload.HeartbeatInterval,
                request.Payload.ReportInterval);

            await _snapshots.UpdateAsync(snapshot, ct);
        }

        anchor.RegisterHeartbeat(eventAt, request.Payload.Network.IpAddress);
        await _anchors.UpdateAsync(anchor, ct);

        raw.MarkProcessed();
        await _rawEvents.UpdateAsync(raw, ct);
        await _uow.SaveChangesAsync(ct);

        return Result<Guid>.Success(configEvent.Id);
    }
}