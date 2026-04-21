using Application.Common.Models;
using Application.DTOs.EventProcessing;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.EventProcessing.Commands;

public sealed record ProcessUwbConfigReportedCommand(UwbConfigReportedPayloadDto Payload) : IRequest<Result<Guid>>;

public sealed class ProcessUwbConfigReportedCommandHandler : IRequestHandler<ProcessUwbConfigReportedCommand, Result<Guid>>
{
    private readonly IRawEventRepository _rawEvents;
    private readonly ITagRepository _tags;
    private readonly IUwbConfigEventRepository _events;
    private readonly ITagUwbConfigSnapshotRepository _snapshots;
    private readonly IUnitOfWork _uow;

    public ProcessUwbConfigReportedCommandHandler(
        IRawEventRepository rawEvents,
        ITagRepository tags,
        IUwbConfigEventRepository events,
        ITagUwbConfigSnapshotRepository snapshots,
        IUnitOfWork uow)
    {
        _rawEvents = rawEvents;
        _tags = tags;
        _events = events;
        _snapshots = snapshots;
        _uow = uow;
    }

    public async Task<Result<Guid>> Handle(ProcessUwbConfigReportedCommand request, CancellationToken ct)
    {
        if (await _rawEvents.ExistsByExternalEventIdAsync(request.Payload.Id, ct))
            return Result<Guid>.Failure("Duplicate raw event.");

        var tag = await _tags.GetByExternalIdAsync(request.Payload.TagId, ct);
        if (tag is null)
            return Result<Guid>.Failure("Tag not found.");

        var eventAt = EventProcessingHelper.FromUnixMilliseconds(request.Payload.Timestamp);

        var raw = RawEvent.Create(
            request.Payload.Id,
            request.Payload.Type,
            eventAt,
            EventProcessingHelper.Serialize(request.Payload),
            request.Payload.TagId,
            null);

        await _rawEvents.AddAsync(raw, ct);

        var configEvent = UwbConfigEvent.Create(
            raw.Id,
            tag.Id,
            eventAt,
            request.Payload.Enabled,
            request.Payload.Channel,
            request.Payload.TxPower,
            request.Payload.RangingInterval,
            request.Payload.TagToTagEnabled,
            request.Payload.TagToTagInterval);

        await _events.AddAsync(configEvent, ct);

        var snapshot = await _snapshots.GetByTagIdAsync(tag.Id, ct);
        if (snapshot is null)
        {
            snapshot = TagUwbConfigSnapshot.Create(
                tag.Id,
                raw.Id,
                eventAt,
                request.Payload.Enabled,
                request.Payload.Channel,
                request.Payload.TxPower,
                request.Payload.RangingInterval,
                request.Payload.TagToTagEnabled,
                request.Payload.TagToTagInterval);

            await _snapshots.AddAsync(snapshot, ct);
        }
        else
        {
            snapshot.UpdateSnapshot(
                raw.Id,
                eventAt,
                request.Payload.Enabled,
                request.Payload.Channel,
                request.Payload.TxPower,
                request.Payload.RangingInterval,
                request.Payload.TagToTagEnabled,
                request.Payload.TagToTagInterval);

            await _snapshots.UpdateAsync(snapshot, ct);
        }

        tag.MarkSeen(eventAt);
        await _tags.UpdateAsync(tag, ct);

        raw.MarkProcessed();
        await _rawEvents.UpdateAsync(raw, ct);
        await _uow.SaveChangesAsync(ct);

        return Result<Guid>.Success(configEvent.Id);
    }
}