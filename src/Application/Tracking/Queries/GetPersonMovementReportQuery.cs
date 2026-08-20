using Application.Common.Models;
using Application.DTOs.Movement;
using Domain.Abstractions;
using MediatR;

namespace Application.Tracking.Queries;

public sealed record GetPersonMovementReportQuery(
    Guid UserId,
    DateTime From,
    DateTime To,

    Guid? CompanyId,
    Guid? BranchId,
    Guid? FloorMapId,
    Guid? FloorMapZoneId,

    int Page = 1,
    int PageSize = 1000
) : IRequest<Result<PersonMovementReportDto>>;

public sealed class GetPersonMovementReportQueryHandler
    : IRequestHandler<
        GetPersonMovementReportQuery,
        Result<PersonMovementReportDto>>
{
    private readonly IMovementEventRepository
        _movementRepository;

    public GetPersonMovementReportQueryHandler(
        IMovementEventRepository movementRepository)
    {
        _movementRepository =
            movementRepository;
    }

    public async Task<Result<PersonMovementReportDto>>
        Handle(
            GetPersonMovementReportQuery request,
            CancellationToken ct)
    {
        if (request.UserId == Guid.Empty)
        {
            return Result<PersonMovementReportDto>
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
            return Result<PersonMovementReportDto>
                .Failure(
                    "'from' and 'to' are required.");
        }

        if (to < from)
        {
            return Result<PersonMovementReportDto>
                .Failure(
                    "'to' cannot be earlier than 'from'.");
        }

        var page =
            Math.Max(
                request.Page,
                1);

        var pageSize =
            Math.Clamp(
                request.PageSize,
                1,
                5000);

        var (items, total) =
            await _movementRepository
                .GetPersonMovementReportAsync(
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

        var resultItems =
            items.Select(x =>
                new PersonMovementReportItemDto(
                    x.Id,
                    x.EventTimestamp,

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

                    x.RecordReason))
            .ToList();

        var first =
            items.FirstOrDefault();

        var totalPages =
            total == 0
                ? 0
                : (int)Math.Ceiling(
                    total /
                    (double)pageSize);

        var dto =
            new PersonMovementReportDto(
                request.UserId,

                first?.UserFullName,
                first?.UserCode,

                from,
                to,

                total,
                page,
                pageSize,
                totalPages,

                resultItems);

        return Result<PersonMovementReportDto>
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