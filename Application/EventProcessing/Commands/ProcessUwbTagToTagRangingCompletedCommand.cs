using Application.Common.Models;
using Application.DTOs.EventProcessing;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.EventProcessing.Commands;

public sealed record ProcessUwbTagToTagRangingCompletedCommand(UwbTagToTagRangingCompletedPayloadDto Payload) : IRequest<Result<Guid>>;

public sealed class ProcessUwbTagToTagRangingCompletedCommandHandler : IRequestHandler<ProcessUwbTagToTagRangingCompletedCommand, Result<Guid>>
{
    private readonly IRawEventRepository _rawEventRepository;
    private readonly ITagRepository _tagRepository;
    private readonly IUwbTagToTagRangingEventRepository _uwbTagToTagRangingEventRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProcessUwbTagToTagRangingCompletedCommandHandler(
        IRawEventRepository rawEventRepository,
        ITagRepository tagRepository,
        IUwbTagToTagRangingEventRepository uwbTagToTagRangingEventRepository,
        IUnitOfWork unitOfWork)
    {
        _rawEventRepository = rawEventRepository;
        _tagRepository = tagRepository;
        _uwbTagToTagRangingEventRepository = uwbTagToTagRangingEventRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(ProcessUwbTagToTagRangingCompletedCommand request, CancellationToken ct)
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

        var rangingEvent = UwbTagToTagRangingEvent.Create(
            rawEvent.Id,
            tag.Id,
            peerTag.Id,
            eventAt,
            request.Payload.Distance);

        await _uwbTagToTagRangingEventRepository.AddAsync(rangingEvent, ct);

        tag.MarkSeen(eventAt);
        peerTag.MarkSeen(eventAt);

        await _tagRepository.UpdateAsync(tag, ct);
        await _tagRepository.UpdateAsync(peerTag, ct);

        rawEvent.MarkProcessed();
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<Guid>.Success(rangingEvent.Id);
    }
}