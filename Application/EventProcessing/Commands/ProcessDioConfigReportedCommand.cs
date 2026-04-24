using Application.Common.Models;
using Application.DTOs.EventProcessing;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.EventProcessing.Commands;

public sealed record ProcessDioConfigReportedCommand(DioConfigReportedPayloadDto Payload) : IRequest<Result<Guid>>;

public sealed class ProcessDioConfigReportedCommandHandler : IRequestHandler<ProcessDioConfigReportedCommand, Result<Guid>>
{
    private readonly IRawEventRepository _rawEvents;
    private readonly ITagRepository _tags;
    private readonly IDioConfigEventRepository _events;
    private readonly ITagDioConfigSnapshotRepository _snapshots;
    private readonly IUnitOfWork _uow;

    public ProcessDioConfigReportedCommandHandler(
        IRawEventRepository rawEvents,
        ITagRepository tags,
        IDioConfigEventRepository events,
        ITagDioConfigSnapshotRepository snapshots,
        IUnitOfWork uow)
    {
        _rawEvents = rawEvents;
        _tags = tags;
        _events = events;
        _snapshots = snapshots;
        _uow = uow;
    }

    public async Task<Result<Guid>> Handle(ProcessDioConfigReportedCommand request, CancellationToken ct)
    {
        if (await _rawEvents.ExistsByExternalEventIdAsync(request.Payload.Id, ct))
            return Result<Guid>.Failure("Duplicate raw event.");

        var tag = await _tags.GetByExternalIdAsync(request.Payload.TagId, ct);
        if (tag is null)
            return Result<Guid>.Failure("Tag not found.");

        var eventAt = EventProcessingHelper.FromUnixMilliseconds(request.Payload.Timestamp);
        var lowPassFilterJson = EventProcessingHelper.Serialize(request.Payload.LowPassFilter);
        var direction = EventProcessingHelper.ParseDioDirection(request.Payload.Direction);

        var raw = RawEvent.Create(
            request.Payload.Id,
            request.Payload.Type,
            eventAt,
            EventProcessingHelper.Serialize(request.Payload),
            request.Payload.TagId,
            null);

        await _rawEvents.AddAsync(raw, ct);

        var configEvent = DioConfigEvent.Create(
            raw.Id,
            tag.Id,
            eventAt,
            request.Payload.Pin,
            direction,
            request.Payload.PeriodicReportEnabled,
            request.Payload.PeriodicReportInterval,
            request.Payload.ReportOnChange,
            lowPassFilterJson);

        await _events.AddAsync(configEvent, ct);

        var snapshot = await _snapshots.GetByTagIdAndPinAsync(tag.Id, request.Payload.Pin, ct);
        if (snapshot is null)
        {
            snapshot = TagDioConfigSnapshot.Create(
                tag.Id,
                request.Payload.Pin,
                raw.Id,
                eventAt,
                direction,
                request.Payload.PeriodicReportEnabled,
                request.Payload.PeriodicReportInterval,
                request.Payload.ReportOnChange,
                lowPassFilterJson);

            await _snapshots.AddAsync(snapshot, ct);
        }
        else
        {
            snapshot.UpdateSnapshot(
                raw.Id,
                eventAt,
                direction,
                request.Payload.PeriodicReportEnabled,
                request.Payload.PeriodicReportInterval,
                request.Payload.ReportOnChange,
                lowPassFilterJson);

            await _snapshots.UpdateAsync(snapshot, ct);
        }

        tag.MarkSeen(eventAt);
        await _tags.UpdateAsync(tag, ct);

        raw.MarkProcessed();
        await _uow.SaveChangesAsync(ct);

        return Result<Guid>.Success(configEvent.Id);
    }
}