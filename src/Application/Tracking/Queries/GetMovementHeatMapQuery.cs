using Application.Common.Models;
using Application.DTOs.Movement;
using Domain.Abstractions;
using MediatR;

namespace Application.Tracking.Queries;

public sealed record GetMovementHeatMapQuery(
    Guid FloorMapId,

    DateTime From,
    DateTime To,

    Guid? UserId,
    Guid? CompanyId,
    Guid? BranchId,
    Guid? FloorMapZoneId,

    decimal GridSize = 1m
) : IRequest<Result<MovementHeatMapDto>>;

public sealed class GetMovementHeatMapQueryHandler
    : IRequestHandler<
        GetMovementHeatMapQuery,
        Result<MovementHeatMapDto>>
{
    private readonly IMovementEventRepository
        _movementRepository;

    private readonly IFloorMapRepository
        _floorMapRepository;

    public GetMovementHeatMapQueryHandler(
        IMovementEventRepository movementRepository,
        IFloorMapRepository floorMapRepository)
    {
        _movementRepository =
            movementRepository;

        _floorMapRepository =
            floorMapRepository;
    }

    public async Task<Result<MovementHeatMapDto>>
        Handle(
            GetMovementHeatMapQuery request,
            CancellationToken ct)
    {
        if (request.FloorMapId == Guid.Empty)
        {
            return Result<MovementHeatMapDto>
                .Failure(
                    "FloorMapId is required.");
        }

        var from =
            EnsureUtc(request.From);

        var to =
            EnsureUtc(request.To);

        if (from == default ||
            to == default)
        {
            return Result<MovementHeatMapDto>
                .Failure(
                    "'from' and 'to' are required.");
        }

        if (to < from)
        {
            return Result<MovementHeatMapDto>
                .Failure(
                    "'to' cannot be earlier than 'from'.");
        }

        if (request.GridSize <= 0)
        {
            return Result<MovementHeatMapDto>
                .Failure(
                    "GridSize must be greater than zero.");
        }

        /*
         * Çok ufak grid yanlışlıkla yüz binlerce
         * heatmap hücresi üretmesin.
         */
        if (request.GridSize < 0.10m)
        {
            return Result<MovementHeatMapDto>
                .Failure(
                    "Minimum GridSize is 0.10.");
        }

        /*
         * Map hem mevcut olmalı hem de
         * repository scope'una dahil olmalı.
         */
        var floorMap =
            await _floorMapRepository
                .GetByIdAsync(
                    request.FloorMapId,
                    ct);

        if (floorMap is null)
        {
            return Result<MovementHeatMapDto>
                .Failure(
                    "Floor map not found or access denied.");
        }

        if (request.FloorMapZoneId.HasValue)
        {
            var zone =
                await _floorMapRepository
                    .GetZoneByIdAsync(
                        request.FloorMapZoneId.Value,
                        ct);

            if (zone is null ||
                zone.FloorMapId !=
                request.FloorMapId)
            {
                return Result<MovementHeatMapDto>
                    .Failure(
                        "Floor map zone not found or does not belong to the selected floor map.");
            }
        }

        var buckets =
            await _movementRepository
                .GetHeatMapAsync(
                    request.FloorMapId,

                    from,
                    to,

                    request.UserId,
                    request.CompanyId,
                    request.BranchId,
                    request.FloorMapZoneId,

                    request.GridSize,

                    ct);

        var maxCount =
            buckets.Count == 0
                ? 0
                : buckets.Max(
                    x => x.Count);

        var totalPointCount =
            buckets.Sum(
                x => x.Count);

        var cells =
            buckets
                .Select(x =>
                {
                    var intensity =
                        maxCount == 0
                            ? 0m
                            : x.Count /
                              (decimal)maxCount;

                    return new MovementHeatMapCellDto(
                        x.X,
                        x.Y,
                        x.Count,
                        decimal.Round(
                            intensity,
                            4));
                })
                .ToList();

        var dto =
            new MovementHeatMapDto(
                request.FloorMapId,

                from,
                to,

                request.UserId,
                request.CompanyId,
                request.BranchId,
                request.FloorMapZoneId,

                request.GridSize,

                totalPointCount,
                maxCount,

                cells);

        return Result<MovementHeatMapDto>
            .Success(dto);
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