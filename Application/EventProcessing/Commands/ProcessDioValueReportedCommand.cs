using Application.Common.Models;
using Application.DTOs.EventProcessing;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.EventProcessing.Commands;

public sealed record ProcessDioValueReportedCommand(DioValueReportedPayloadDto Payload) : IRequest<Result<Guid>>;

public sealed class ProcessDioValueReportedCommandHandler : IRequestHandler<ProcessDioValueReportedCommand, Result<Guid>>
{
    private readonly IRawEventRepository _rawEventRepository;
    private readonly ITagRepository _tagRepository;
    private readonly IDioValueEventRepository _dioValueEventRepository;
    private readonly ITagDioValueSnapshotRepository _tagDioValueSnapshotRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProcessDioValueReportedCommandHandler(
        IRawEventRepository rawEventRepository,
        ITagRepository tagRepository,
        IDioValueEventRepository dioValueEventRepository,
        ITagDioValueSnapshotRepository tagDioValueSnapshotRepository,
        IUnitOfWork unitOfWork)
    {
        _rawEventRepository = rawEventRepository;
        _tagRepository = tagRepository;
        _dioValueEventRepository = dioValueEventRepository;
        _tagDioValueSnapshotRepository = tagDioValueSnapshotRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(ProcessDioValueReportedCommand request, CancellationToken ct)
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
            rawEvent.MarkFailed("Tag not found.");
            await _unitOfWork.SaveChangesAsync(ct);
            return Result<Guid>.Failure("Tag not found.");
        }

        var telemetryEvent = DioValueEvent.Create(
            rawEvent.Id,
            tag.Id,
            eventAt,
            request.Payload.Pin,
            request.Payload.Value);

        await _dioValueEventRepository.AddAsync(telemetryEvent, ct);

        var snapshot = await _tagDioValueSnapshotRepository.GetByTagIdAndPinAsync(tag.Id, request.Payload.Pin, ct);
        if (snapshot is null)
        {
            snapshot = TagDioValueSnapshot.Create(
                tag.Id,
                request.Payload.Pin,
                rawEvent.Id,
                eventAt,
                request.Payload.Value);

            await _tagDioValueSnapshotRepository.AddAsync(snapshot, ct);
        }
        else
        {
            snapshot.UpdateSnapshot(
                rawEvent.Id,
                eventAt,
                request.Payload.Value);

            await _tagDioValueSnapshotRepository.UpdateAsync(snapshot, ct);
        }

        tag.MarkSeen(eventAt);
        await _tagRepository.UpdateAsync(tag, ct);

        rawEvent.MarkProcessed();
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<Guid>.Success(telemetryEvent.Id);
    }
}