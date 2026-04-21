using Application.Common.Models;
using Application.DTOs.EventProcessing;
using Domain.Abstractions;
using MediatR;

namespace Application.EventProcessing.Queries;

public sealed record GetAnchorHeartbeatEventsByAnchorIdQuery(
    Guid AnchorId,
    int Page = 1,
    int PageSize = 100
) : IRequest<Result<IReadOnlyList<AnchorHeartbeatEventDto>>>;

public sealed class GetAnchorHeartbeatEventsByAnchorIdQueryHandler : IRequestHandler<GetAnchorHeartbeatEventsByAnchorIdQuery, Result<IReadOnlyList<AnchorHeartbeatEventDto>>>
{
    private readonly IAnchorHeartbeatEventRepository _anchorHeartbeatEventRepository;

    public GetAnchorHeartbeatEventsByAnchorIdQueryHandler(IAnchorHeartbeatEventRepository anchorHeartbeatEventRepository)
    {
        _anchorHeartbeatEventRepository = anchorHeartbeatEventRepository;
    }

    public async Task<Result<IReadOnlyList<AnchorHeartbeatEventDto>>> Handle(GetAnchorHeartbeatEventsByAnchorIdQuery request, CancellationToken ct)
    {
        var (items, _) = await _anchorHeartbeatEventRepository.GetPagedByAnchorIdAsync(
            request.AnchorId,
            request.Page,
            request.PageSize,
            ct);

        var dto = items
            .Select(x => new AnchorHeartbeatEventDto(
                x.Id,
                x.RawEventId,
                x.AnchorId,
                x.EventTimestamp,
                x.IpAddress))
            .ToList();

        return Result<IReadOnlyList<AnchorHeartbeatEventDto>>.Success(dto);
    }
}