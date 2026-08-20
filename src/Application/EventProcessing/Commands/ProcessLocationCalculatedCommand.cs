using Application.Common.Maps;
using Application.Common.Models;
using Application.Common.Realtime;
using Application.Common.Mappings;
using Application.Common.Movement;
using Application.DTOs.EventProcessing;
using Application.DTOs.Tracking;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.EventProcessing.Commands;

public sealed record ProcessLocationCalculatedCommand(
    LocationCalculatedPayloadDto Payload)
    : IRequest<Result<Guid>>;

public sealed class ProcessLocationCalculatedCommandHandler
    : IRequestHandler<
        ProcessLocationCalculatedCommand,
        Result<Guid>>
{
    private readonly IRawEventRepository _rawEventRepository;
    private readonly ITagRepository _tagRepository;
    private readonly ILocationEventRepository _locationEventRepository;
    private readonly ICurrentLocationRepository _currentLocationRepository;
    private readonly IMovementEventRepository _movementEventRepository;
    private readonly IMovementRecordingPolicy _movementRecordingPolicy;
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
        IMovementEventRepository movementEventRepository,
        IMovementRecordingPolicy movementRecordingPolicy,
        ITagAssignmentRepository tagAssignmentRepository,
        IFloorMapRepository floorMapRepository,
        IMapCoordinateTransformer coordinateTransformer,
        IMapZoneResolver zoneResolver,
        IUnitOfWork unitOfWork,
        IRealtimeNotifier realtimeNotifier)
    {
        _rawEventRepository =
            rawEventRepository;

        _tagRepository =
            tagRepository;

        _locationEventRepository =
            locationEventRepository;

        _currentLocationRepository =
            currentLocationRepository;

        _movementEventRepository =
            movementEventRepository;

        _movementRecordingPolicy =
            movementRecordingPolicy;

        _tagAssignmentRepository =
            tagAssignmentRepository;

        _floorMapRepository =
            floorMapRepository;

        _coordinateTransformer =
            coordinateTransformer;

        _zoneResolver =
            zoneResolver;

        _unitOfWork =
            unitOfWork;

        _realtimeNotifier =
            realtimeNotifier;
    }

    public async Task<Result<Guid>> Handle(
        ProcessLocationCalculatedCommand request,
        CancellationToken ct)
    {
        // ============================================================
        // DUPLICATE CONTROL
        // ============================================================

        if (await _rawEventRepository
            .ExistsByExternalEventIdAsync(
                request.Payload.Id,
                ct))
        {
            return Result<Guid>.Failure(
                "Duplicate raw event.");
        }

        var eventAt =
            EventProcessingHelper
                .FromUnixMilliseconds(
                    request.Payload.Timestamp);

        // ============================================================
        // RAW EVENT
        // ============================================================

        var rawEvent =
            RawEvent.Create(
                request.Payload.Id,
                request.Payload.Type,
                eventAt,
                EventProcessingHelper.Serialize(
                    request.Payload),
                request.Payload.TagId,
                null);

        await _rawEventRepository.AddAsync(
            rawEvent,
            ct);

        // ============================================================
        // TAG
        // ============================================================

        var tag =
            await _tagRepository
                .GetByExternalIdAsync(
                    request.Payload.TagId,
                    ct);

        if (tag is null)
        {
            rawEvent.MarkFailed(
                "Tag not found.");

            await _unitOfWork
                .SaveChangesAsync(ct);

            return Result<Guid>.Failure(
                "Tag not found.");
        }

        // ============================================================
        // USED ANCHORS
        // ============================================================

        var usedAnchorsJson =
            EventProcessingHelper.Serialize(
                request.Payload.UsedAnchors);

        var anchorCount =
            request.Payload.UsedAnchors?.Count ?? 0;

        // ============================================================
        // FLOOR MAP
        // ============================================================

        var floorMap =
            await ResolveFloorMapAsync(
                request.Payload,
                ct);

        FloorMapCalibration? calibration = null;

        if (floorMap is not null)
        {
            calibration =
                await _floorMapRepository
                    .GetDefaultCalibrationAsync(
                        floorMap.Id,
                        ct);
        }

        // ============================================================
        // COORDINATE TRANSFORMATION
        // ============================================================

        var mapX =
            request.Payload.X;

        var mapY =
            request.Payload.Y;

        var mapZ =
            request.Payload.Z;

        if (calibration is not null)
        {
            var mapped =
                _coordinateTransformer
                    .TransformToMap(
                        calibration,
                        request.Payload.X,
                        request.Payload.Y,
                        request.Payload.Z);

            mapX = mapped.X;
            mapY = mapped.Y;
            mapZ = mapped.Z;
        }

        var floorMapId =
            floorMap?.Id;

        Guid? floorMapZoneId = null;

        if (floorMap is not null)
        {
            var zones =
                await _floorMapRepository
                    .GetZonesAsync(
                        floorMap.Id,
                        ct);

            floorMapZoneId =
                _zoneResolver.ResolveZoneId(
                    zones,
                    mapX,
                    mapY);
        }

        // ============================================================
        // RAW LOCATION HISTORY
        // ============================================================

        var locationEvent =
            LocationEvent.Create(
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

        await _locationEventRepository
            .AddAsync(
                locationEvent,
                ct);

        // ============================================================
        // ACTIVE TAG ASSIGNMENT
        // ============================================================

        var activeAssignment =
            await _tagAssignmentRepository
                .GetActiveByTagIdAsync(
                    tag.Id,
                    ct);

        var assignedUser =
            activeAssignment?.User;

        // ============================================================
        // CURRENT LOCATION
        // ============================================================

        var currentLocation =
            await _currentLocationRepository
                .GetByTagIdAsync(
                    tag.Id,
                    ct);

        // ============================================================
        // LAST MOVEMENT POINT
        // ============================================================

        var previousMovement =
            await _movementEventRepository
                .GetLastByTagIdAsync(
                    tag.Id,
                    ct);

        // ============================================================
        // MOVEMENT RECORDING DECISION
        // ============================================================

        var movementDecision =
            _movementRecordingPolicy
                .Evaluate(
                    previousMovement,
                    floorMapId,
                    floorMapZoneId,
                    mapX,
                    mapY,
                    mapZ,
                    eventAt);

        // ============================================================
        // MOVEMENT HISTORY SNAPSHOT
        // ============================================================

        if (movementDecision.ShouldRecord)
        {
            var movementCompanyId =
                tag.CompanyId ??
                floorMap?.CompanyId;

            var movementBranchId =
                tag.BranchId ??
                floorMap?.BranchId;

            string? userCode = null;

            if (assignedUser is not null &&
                movementCompanyId.HasValue)
            {
                userCode =
                    assignedUser.UserCompanies
                        .FirstOrDefault(x =>
                            x.CompanyId ==
                            movementCompanyId.Value)
                        ?.UserCode;
            }

            var userFullName =
                assignedUser is null
                    ? null
                    : $"{assignedUser.FirstName} {assignedUser.LastName}"
                        .Trim();

            var movementEvent =
                MovementEvent.Create(
                    rawEvent.Id,

                    tag.Id,
                    tag.ExternalId,
                    tag.Code,
                    tag.TagType.ToString(),

                    activeAssignment?.UserId,
                    userFullName,
                    userCode,

                    movementCompanyId,
                    movementBranchId,

                    floorMapId,
                    floorMapZoneId,

                    mapX,
                    mapY,
                    mapZ,

                    request.Payload.Accuracy,
                    request.Payload.Confidence,

                    eventAt,

                    movementDecision.Reason ??
                    "Unknown");

            await _movementEventRepository
                .AddAsync(
                    movementEvent,
                    ct);
        }

        // ============================================================
        // TAG STATUS
        // ============================================================

        tag.MarkSeen(eventAt);

        await _tagRepository
            .UpdateAsync(
                tag,
                ct);

        // ============================================================
        // CURRENT LOCATION PROJECTION
        // ============================================================

        var isCurrentProjectionUpdated =
            false;

        if (currentLocation is null)
        {
            currentLocation =
                CurrentLocation.Create(
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

            await _currentLocationRepository
                .AddAsync(
                    currentLocation,
                    ct);

            isCurrentProjectionUpdated =
                true;
        }
        else if (
            eventAt >=
            currentLocation.LastEventAt)
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

            await _currentLocationRepository
                .UpdateAsync(
                    currentLocation,
                    ct);

            isCurrentProjectionUpdated =
                true;
        }

        // ============================================================
        // RAW EVENT PROCESSED
        // ============================================================

        rawEvent.MarkProcessed();

        // LocationEvent + MovementEvent + CurrentLocation +
        // Tag + RawEvent ayný transaction/save içerisinde kaydedilir.

        await _unitOfWork
            .SaveChangesAsync(ct);

        // ============================================================
        // SIGNALR
        // ============================================================

        if (isCurrentProjectionUpdated)
        {
            var currentLocationPayload =
                new CurrentLocationDto(
                    currentLocation.Id,

                    tag.Id,
                    tag.ExternalId,
                    tag.Code,
                    tag.TagType.ToString(),

                    assignedUser?.Id,
                    assignedUser.GetFullName(),
                    assignedUser?.Identifier,

                    currentLocation.FloorMapId,
                    currentLocation.FloorMapZoneId,

                    currentLocation.X,
                    currentLocation.Y,
                    currentLocation.Z,

                    currentLocation.Accuracy,
                    currentLocation.Confidence,

                    currentLocation.LastEventAt,
                    currentLocation.LastRawEventId,
                    currentLocation.LastKnownAnchorCount);

            await _realtimeNotifier
                .LocationUpdatedAsync(
                    currentLocationPayload,
                    ct);

            var tagStatusPayload =
                new TagStatusChangedRealtimeDto(
                    tag.Id,
                    tag.ExternalId,
                    tag.Code,
                    tag.TagType.ToString(),

                    assignedUser?.Id,
                    assignedUser.GetFullName(),
                    assignedUser?.Identifier,

                    tag.Status.ToString(),
                    eventAt);

            await _realtimeNotifier
                .TagStatusChangedAsync(
                    tagStatusPayload,
                    ct);
        }

        return Result<Guid>.Success(
            locationEvent.Id);
    }

    private async Task<FloorMap?> ResolveFloorMapAsync(
        LocationCalculatedPayloadDto payload,
        CancellationToken ct)
    {
        // Payload doðrudan FloorMapId gönderdiyse
        if (payload.FloorMapId.HasValue)
        {
            var map =
                await _floorMapRepository
                    .GetByIdAsync(
                        payload.FloorMapId.Value,
                        ct);

            if (map is not null &&
                map.IsActive)
            {
                return map;
            }
        }

        // FloorMapId yoksa kullanýlan anchor'lardan
        // aktif haritayý bul.
        var usedAnchorIds =
            payload.UsedAnchors?
                .Select(x => x.AnchorId)
                .Where(x =>
                    !string.IsNullOrWhiteSpace(x))
                .ToList()
            ?? new List<string>();

        if (usedAnchorIds.Count > 0)
        {
            var map =
                await _floorMapRepository
                    .GetActiveMapByUsedAnchorExternalIdsAsync(
                        usedAnchorIds,
                        ct);

            if (map is not null)
                return map;
        }

        return null;
    }
}