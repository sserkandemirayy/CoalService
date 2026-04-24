using Application.Common.Models;
using Application.DTOs.EventProcessing;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.EventProcessing.Commands;

public sealed record ProcessUwbRangingCompletedCommand(UwbRangingCompletedPayloadDto Payload) : IRequest<Result<Guid>>;

public sealed class ProcessUwbRangingCompletedCommandHandler : IRequestHandler<ProcessUwbRangingCompletedCommand, Result<Guid>>
{
    private readonly IRawEventRepository _rawEventRepository;
    private readonly IAnchorRepository _anchorRepository;
    private readonly ITagRepository _tagRepository;
    private readonly IUwbRangingEventRepository _uwbRangingEventRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProcessUwbRangingCompletedCommandHandler(
        IRawEventRepository rawEventRepository,
        IAnchorRepository anchorRepository,
        ITagRepository tagRepository,
        IUwbRangingEventRepository uwbRangingEventRepository,
        IUnitOfWork unitOfWork)
    {
        _rawEventRepository = rawEventRepository;
        _anchorRepository = anchorRepository;
        _tagRepository = tagRepository;
        _uwbRangingEventRepository = uwbRangingEventRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(ProcessUwbRangingCompletedCommand request, CancellationToken ct)
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

        var anchor = await _anchorRepository.GetByExternalIdAsync(request.Payload.AnchorId, ct);
        if (anchor is null)
        {
            rawEvent.MarkFailed("Anchor not found.");
            await _unitOfWork.SaveChangesAsync(ct);
            return Result<Guid>.Failure("Anchor not found.");
        }

        var tag = await _tagRepository.GetByExternalIdAsync(request.Payload.TagId, ct);
        if (tag is null)
        {
            rawEvent.MarkFailed("Tag not found.");
            await _unitOfWork.SaveChangesAsync(ct);
            return Result<Guid>.Failure("Tag not found.");
        }

        var rangingEvent = UwbRangingEvent.Create(
            rawEvent.Id,
            anchor.Id,
            tag.Id,
            eventAt,
            request.Payload.Distance);

        await _uwbRangingEventRepository.AddAsync(rangingEvent, ct);

        tag.MarkSeen(eventAt);
        await _tagRepository.UpdateAsync(tag, ct);

        anchor.RegisterHeartbeat(eventAt);
        await _anchorRepository.UpdateAsync(anchor, ct);

        rawEvent.MarkProcessed();
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<Guid>.Success(rangingEvent.Id);
    }
}