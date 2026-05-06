using Application.Common.Models;
using Application.DTOs.EventProcessing;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.EventProcessing.Commands;

public sealed record ProcessTagDataReceivedCommand(TagDataReceivedPayloadDto Payload) : IRequest<Result<Guid>>;

public sealed class ProcessTagDataReceivedCommandHandler : IRequestHandler<ProcessTagDataReceivedCommand, Result<Guid>>
{
    private readonly IRawEventRepository _rawEventRepository;
    private readonly IAnchorRepository _anchorRepository;
    private readonly ITagRepository _tagRepository;
    private readonly ITagDataEventRepository _tagDataEventRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProcessTagDataReceivedCommandHandler(
        IRawEventRepository rawEventRepository,
        IAnchorRepository anchorRepository,
        ITagRepository tagRepository,
        ITagDataEventRepository tagDataEventRepository,
        IUnitOfWork unitOfWork)
    {
        _rawEventRepository = rawEventRepository;
        _anchorRepository = anchorRepository;
        _tagRepository = tagRepository;
        _tagDataEventRepository = tagDataEventRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(ProcessTagDataReceivedCommand request, CancellationToken ct)
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

        var tagDataEvent = TagDataEvent.Create(
            rawEvent.Id,
            anchor.Id,
            tag.Id,
            eventAt,
            request.Payload.TagType);

        await _tagDataEventRepository.AddAsync(tagDataEvent, ct);

        tag.MarkSeen(eventAt);
        await _tagRepository.UpdateAsync(tag, ct);

        anchor.RegisterHeartbeat(eventAt);
        await _anchorRepository.UpdateAsync(anchor, ct);

        rawEvent.MarkProcessed();
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<Guid>.Success(tagDataEvent.Id);
    }
}