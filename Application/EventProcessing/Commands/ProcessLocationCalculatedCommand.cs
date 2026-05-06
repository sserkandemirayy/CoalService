using Application.Common.Models;
using Application.DTOs.EventProcessing;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.EventProcessing.Commands;

public sealed record ProcessLocationCalculatedCommand(LocationCalculatedPayloadDto Payload) : IRequest<Result<Guid>>;

public sealed class ProcessLocationCalculatedCommandHandler : IRequestHandler<ProcessLocationCalculatedCommand, Result<Guid>>
{
    private readonly IRawEventRepository _rawEventRepository;
    private readonly ITagRepository _tagRepository;
    private readonly ILocationEventRepository _locationEventRepository;
    private readonly ICurrentLocationRepository _currentLocationRepository;
    private readonly ITagAssignmentRepository _tagAssignmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProcessLocationCalculatedCommandHandler(
        IRawEventRepository rawEventRepository,
        ITagRepository tagRepository,
        ILocationEventRepository locationEventRepository,
        ICurrentLocationRepository currentLocationRepository,
        ITagAssignmentRepository tagAssignmentRepository,
        IUnitOfWork unitOfWork)
    {
        _rawEventRepository = rawEventRepository;
        _tagRepository = tagRepository;
        _locationEventRepository = locationEventRepository;
        _currentLocationRepository = currentLocationRepository;
        _tagAssignmentRepository = tagAssignmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(ProcessLocationCalculatedCommand request, CancellationToken ct)
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

        var usedAnchorsJson = EventProcessingHelper.Serialize(request.Payload.UsedAnchors);

        var locationEvent = LocationEvent.Create(
            rawEvent.Id,
            tag.Id,
            eventAt,
            request.Payload.X,
            request.Payload.Y,
            request.Payload.Z,
            request.Payload.Accuracy,
            request.Payload.Confidence,
            usedAnchorsJson);

        await _locationEventRepository.AddAsync(locationEvent, ct);

        tag.MarkSeen(eventAt);
        await _tagRepository.UpdateAsync(tag, ct);

        var activeAssignment = await _tagAssignmentRepository.GetActiveByTagIdAsync(tag.Id, ct);
        var currentLocation = await _currentLocationRepository.GetByTagIdAsync(tag.Id, ct);
        var anchorCount = request.Payload.UsedAnchors?.Count ?? 0;

        if (currentLocation is null)
        {
            currentLocation = CurrentLocation.Create(
                tag.Id,
                activeAssignment?.UserId,
                request.Payload.X,
                request.Payload.Y,
                request.Payload.Z,
                request.Payload.Accuracy,
                request.Payload.Confidence,
                eventAt,
                rawEvent.Id,
                anchorCount);

            await _currentLocationRepository.AddAsync(currentLocation, ct);
        }
        else
        {
            currentLocation.UpdateFromLocation(
                activeAssignment?.UserId,
                request.Payload.X,
                request.Payload.Y,
                request.Payload.Z,
                request.Payload.Accuracy,
                request.Payload.Confidence,
                eventAt,
                rawEvent.Id,
                anchorCount);

            await _currentLocationRepository.UpdateAsync(currentLocation, ct);
        }

        rawEvent.MarkProcessed();
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<Guid>.Success(locationEvent.Id);
    }
}