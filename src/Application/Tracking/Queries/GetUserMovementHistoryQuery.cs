using Application.Common.Models;
using Application.DTOs.Tracking;
using Domain.Abstractions;
using MediatR;

namespace Application.Tracking.Queries;

public sealed record GetUserMovementHistoryQuery(
    Guid UserId,
    DateTime From,
    DateTime To,
    Guid? CompanyId,
    Guid? BranchId,
    Guid? FloorMapId,
    Guid? FloorMapZoneId,
    int Page = 1,
    int PageSize = 1000
) : IRequest<Result<MovementReportDto>>;

public sealed class GetUserMovementHistoryQueryHandler
    : IRequestHandler<
        GetUserMovementHistoryQuery,
        Result<MovementReportDto>>
{
    private const int MaximumPageSize = 5000;

    private readonly IMovementEventRepository
        _movementEventRepository;

    public GetUserMovementHistoryQueryHandler(
        IMovementEventRepository movementEventRepository)
    {
        _movementEventRepository =
            movementEventRepository;
    }

    public async Task<Result<MovementReportDto>> Handle(
        GetUserMovementHistoryQuery request,
        CancellationToken ct)
    {
        if (request.UserId == Guid.Empty)
        {
            return Result<MovementReportDto>.Failure(
                "UserId is required.");
        }

        var from = EnsureUtc(request.From);
        var to = EnsureUtc(request.To);

        if (from > to)
        {
            return Result<MovementReportDto>.Failure(
                "'from' cannot be greater than 'to'.");
        }

        var page =
            request.Page < 1
                ? 1
                : request.Page;

        var pageSize =
            request.PageSize < 1
                ? 1000
                : Math.Min(
                    request.PageSize,
                    MaximumPageSize);

        var (items, total) =
            await _movementEventRepository
                .GetUserMovementHistoryAsync(
                    request.UserId,
                    from,
                    to,
                    request.CompanyId,
                    request.BranchId,
                    request.FloorMapId,
                    request.FloorMapZoneId,
                    page,
                    pageSize,
                    ct);

        var resultItems = items
            .Select(x => new MovementPointDto(
                x.Id,
                x.RawEventId,

                x.TagId,
                x.TagExternalId,
                x.TagCode,
                x.TagType,

                x.UserId,
                x.UserFullName,
                x.UserCode,

                x.CompanyId,
                x.BranchId,

                x.FloorMapId,
                x.FloorMapZoneId,

                x.X,
                x.Y,
                x.Z,

                x.Accuracy,
                x.Confidence,

                x.EventTimestamp,
                x.RecordReason))
            .ToList();

        var dto = new MovementReportDto(
            request.UserId,
            from,
            to,
            page,
            pageSize,
            total,
            resultItems);

        return Result<MovementReportDto>.Success(dto);
    }

    private static DateTime EnsureUtc(
        DateTime value)
    {
        if (value.Kind == DateTimeKind.Utc)
            return value;

        return DateTime.SpecifyKind(
            value,
            DateTimeKind.Utc);
    }
}