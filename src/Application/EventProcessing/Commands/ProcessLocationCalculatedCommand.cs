using Application.Common.Maps;
using Application.Common.Models;
using Application.Common.Realtime;
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
    private readonly IFloorMapRepository _floorMapRepository;
    private readonly IMapCoordinateTransformer _coordinateTransformer;
    private readonly IMapZoneResolver _zoneResolver;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRealtimeNotifier _realtimeNotifier;

    public ProcessLocationCalculatedCommandHandler(
        IRawEventRepository rawEventRepository,
        ITagRepository tagRepository,
        ILocationEventRepository locationEventRepository,
        ICurrentLocationRepository currentLocationRepository,
        ITagAssignmentRepository tagAssignmentRepository,
        IFloorMapRepository floorMapRepository,
        IMapCoordinateTransformer coordinateTransformer,
        IMapZoneResolver zoneResolver,
        IUnitOfWork unitOfWork,
        IRealtimeNotifier realtimeNotifier)
    {
        _rawEventRepository = rawEventRepository;
        _tagRepository = tagRepository;
        _locationEventRepository = locationEventRepository;
        _currentLocationRepository = currentLocationRepository;
        _tagAssignmentRepository = tagAssignmentRepository;
        _floorMapRepository = floorMapRepository;
        _coordinateTransformer = coordinateTransformer;
        _zoneResolver = zoneResolver;
        _unitOfWork = unitOfWork;
        _realtimeNotifier = realtimeNotifier;
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
        var anchorCount = request.Payload.UsedAnchors?.Count ?? 0;

        var floorMap = await ResolveFloorMapAsync(request.Payload, ct);
        FloorMapCalibration? calibration = null;

        if (floorMap is not null)
            calibration = await _floorMapRepository.GetDefaultCalibrationAsync(floorMap.Id, ct);

        var mapX = request.Payload.X;
        var mapY = request.Payload.Y;
        var mapZ = request.Payload.Z;

        if (calibration is not null)
        {
            var mapped = _coordinateTransformer.TransformToMap(
                calibration,
                request.Payload.X,
                request.Payload.Y,
                request.Payload.Z);

            mapX = mapped.X;
            mapY = mapped.Y;
            mapZ = mapped.Z;
        }

        Guid? floorMapId = floorMap?.Id;
        Guid? floorMapZoneId = null;

        if (floorMap is not null)
        {
            var zones = await _floorMapRepository.GetZonesAsync(floorMap.Id, ct);
            floorMapZoneId = _zoneResolver.ResolveZoneId(zones, mapX, mapY);
        }

        var locationEvent = LocationEvent.Create(
            rawEvent.Id,
            tag.Id,
            eventAt,
            mapX,
            mapY,
            mapZ,
            request.Payload.Accuracy,
            request.Payload.Confidence,
            usedAnchorsJson,
            floorMapId,
            floorMapZoneId);

        await _locationEventRepository.AddAsync(locationEvent, ct);

        tag.MarkSeen(eventAt);
        await _tagRepository.UpdateAsync(tag, ct);

        var activeAssignment = await _tagAssignmentRepository.GetActiveByTagIdAsync(tag.Id, ct);
        var currentLocation = await _currentLocationRepository.GetByTagIdAsync(tag.Id, ct);
        var isCurrentProjectionUpdated = false;

        if (currentLocation is null)
        {
            currentLocation = CurrentLocation.Create(
                tag.Id,
                activeAssignment?.UserId,
                mapX,
                mapY,
                mapZ,
                request.Payload.Accuracy,
                request.Payload.Confidence,
                eventAt,
                rawEvent.Id,
                anchorCount,
                floorMapId,
                floorMapZoneId);

            await _currentLocationRepository.AddAsync(currentLocation, ct);
            isCurrentProjectionUpdated = true;
        }
        else if (eventAt >= currentLocation.LastEventAt)
        {
            currentLocation.UpdateFromLocation(
                activeAssignment?.UserId,
                mapX,
                mapY,
                mapZ,
                request.Payload.Accuracy,
                request.Payload.Confidence,
                eventAt,
                rawEvent.Id,
                anchorCount,
                floorMapId,
                floorMapZoneId);

            await _currentLocationRepository.UpdateAsync(currentLocation, ct);
            isCurrentProjectionUpdated = true;
        }

        rawEvent.MarkProcessed();
        await _unitOfWork.SaveChangesAsync(ct);

        if (isCurrentProjectionUpdated)
        {
            await _realtimeNotifier.LocationUpdatedAsync(
                new LocationUpdatedRealtimeDto(
                    tag.Id,
                    tag.ExternalId,
                    tag.Code,
                    activeAssignment?.UserId,
                    floorMapId,
                    floorMapZoneId,
                    mapX,
                    mapY,
                    mapZ,
                    request.Payload.Accuracy,
                    request.Payload.Confidence,
                    eventAt,
                    anchorCount),
                ct);

            await _realtimeNotifier.TagStatusChangedAsync(
                new TagStatusChangedRealtimeDto(
                    tag.Id,
                    tag.ExternalId,
                    tag.Code,
                    tag.Status.ToString(),
                    eventAt),
                ct);
        }

        return Result<Guid>.Success(locationEvent.Id);
    }

    private async Task<FloorMap?> ResolveFloorMapAsync(
        LocationCalculatedPayloadDto payload,
        CancellationToken ct)
    {
        if (payload.FloorMapId.HasValue)
        {
            var map = await _floorMapRepository.GetByIdAsync(payload.FloorMapId.Value, ct);

            if (map is not null && map.IsActive)
                return map;
        }

        var usedAnchorIds = payload.UsedAnchors?
            .Select(x => x.AnchorId)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList() ?? new List<string>();

        if (usedAnchorIds.Count > 0)
        {
            var map = await _floorMapRepository.GetActiveMapByUsedAnchorExternalIdsAsync(usedAnchorIds, ct);

            if (map is not null)
                return map;
        }

        return null;
    }
}