using Application.Common.Models;
using Application.DTOs.EventProcessing;
using Domain.Abstractions;
using MediatR;

namespace Application.EventProcessing.Queries;

public sealed record GetAnchorConfigEventsByAnchorIdQuery(Guid AnchorId, int Page = 1, int PageSize = 100)
    : IRequest<Result<IReadOnlyList<AnchorConfigEventDto>>>;

public sealed class GetAnchorConfigEventsByAnchorIdQueryHandler
    : IRequestHandler<GetAnchorConfigEventsByAnchorIdQuery, Result<IReadOnlyList<AnchorConfigEventDto>>>
{
    private readonly IAnchorConfigEventRepository _repo;

    public GetAnchorConfigEventsByAnchorIdQueryHandler(IAnchorConfigEventRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<IReadOnlyList<AnchorConfigEventDto>>> Handle(GetAnchorConfigEventsByAnchorIdQuery request, CancellationToken ct)
    {
        var (items, _) = await _repo.GetPagedByAnchorIdAsync(request.AnchorId, request.Page, request.PageSize, ct);

        var dto = items.Select(x => new AnchorConfigEventDto(
            x.Id, x.RawEventId, x.AnchorId, x.EventTimestamp, x.FirmwareVersion,
            x.PositionJson, x.NetworkJson, x.UwbJson, x.BleJson,
            x.HeartbeatInterval, x.ReportInterval)).ToList();

        return Result<IReadOnlyList<AnchorConfigEventDto>>.Success(dto);
    }
}