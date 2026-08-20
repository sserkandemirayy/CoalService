using Application.Common.Models;
using Application.DTOs.Movement;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.Tracking.Queries;

public sealed record GetMovementPlaybackQuery(
    Guid UserId,
    DateTime From,
    DateTime To,

    Guid? CompanyId,
    Guid? BranchId,
    Guid? FloorMapId,

    int MaxPoints = 50000
) : IRequest<Result<MovementPlaybackDto>>;

public sealed class GetMovementPlaybackQueryHandler
    : IRequestHandler<
        GetMovementPlaybackQuery,
        Result<MovementPlaybackDto>>
{
    private const int AbsoluteMaximumPoints =
        100000;

    private readonly IMovementEventRepository
        _movementRepository;

    public GetMovementPlaybackQueryHandler(
        IMovementEventRepository movementRepository)
    {
        _movementRepository =
            movementRepository;
    }

    public async Task<Result<MovementPlaybackDto>>
        Handle(
            GetMovementPlaybackQuery request,
            CancellationToken ct)
    {
        if (request.UserId == Guid.Empty)
        {
            return Result<MovementPlaybackDto>
                .Failure(
                    "UserId is required.");
        }

        var from =
            EnsureUtc(request.From);

        var to =
            EnsureUtc(request.To);

        if (from == default ||
            to == default)
        {
            return Result<MovementPlaybackDto>
                .Failure(
                    "'from' and 'to' are required.");
        }

        if (to < from)
        {
            return Result<MovementPlaybackDto>
                .Failure(
                    "'to' cannot be earlier than 'from'.");
        }

        var requestedMaxPoints =
            Math.Clamp(
                request.MaxPoints,
                1,
                AbsoluteMaximumPoints);

        /*
         * Bir fazla kayıt istiyoruz.
         * Böylece result gerçekten kesildi mi
         * anlayabiliyoruz.
         */
        var source =
            await _movementRepository
                .GetPlaybackAsync(
                    request.UserId,
                    from,
                    to,

                    request.CompanyId,
                    request.BranchId,
                    request.FloorMapId,

                    requestedMaxPoints + 1,

                    ct);

        var isTruncated =
            source.Count >
            requestedMaxPoints;

        var points =
            source
                .Take(requestedMaxPoints)
                .ToList();

        var segments =
            BuildSegments(points);

        var first =
            points.FirstOrDefault();

        var dto =
            new MovementPlaybackDto(
                request.UserId,

                first?.UserFullName,
                first?.UserCode,

                from,
                to,

                points.Count,

                isTruncated,

                segments);

        return Result<MovementPlaybackDto>
            .Success(dto);
    }

    private static IReadOnlyList<MovementPlaybackSegmentDto>
        BuildSegments(
            IReadOnlyList<MovementEvent> movements)
    {
        var result =
            new List<MovementPlaybackSegmentDto>();

        if (movements.Count == 0)
            return result;

        Guid? currentFloorMapId =
            movements[0].FloorMapId;

        var currentPoints =
            new List<MovementPlaybackPointDto>();

        foreach (var movement in movements)
        {
            /*
             * Kat değiştiyse yeni segment.
             *
             * Böylece frontend farklı katlardaki
             * koordinatları düz çizgiyle birbirine
             * bağlamaz.
             */
            if (currentPoints.Count > 0 &&
                movement.FloorMapId !=
                currentFloorMapId)
            {
                result.Add(
                    CreateSegment(
                        currentFloorMapId,
                        currentPoints));

                currentPoints =
                    new List<MovementPlaybackPointDto>();

                currentFloorMapId =
                    movement.FloorMapId;
            }

            currentPoints.Add(
                new MovementPlaybackPointDto(
                    movement.Id,
                    movement.EventTimestamp,

                    movement.X,
                    movement.Y,
                    movement.Z,

                    movement.Accuracy,
                    movement.Confidence,

                    movement.FloorMapZoneId,

                    movement.RecordReason));
        }

        if (currentPoints.Count > 0)
        {
            result.Add(
                CreateSegment(
                    currentFloorMapId,
                    currentPoints));
        }

        return result;
    }

    private static MovementPlaybackSegmentDto
        CreateSegment(
            Guid? floorMapId,
            IReadOnlyList<MovementPlaybackPointDto> points)
    {
        return new MovementPlaybackSegmentDto(
            floorMapId,

            points[0].Timestamp,
            points[^1].Timestamp,

            points.Count,

            points);
    }

    private static DateTime EnsureUtc(
        DateTime value)
    {
        if (value.Kind == DateTimeKind.Utc)
            return value;

        if (value.Kind == DateTimeKind.Local)
            return value.ToUniversalTime();

        return DateTime.SpecifyKind(
            value,
            DateTimeKind.Utc);
    }
}