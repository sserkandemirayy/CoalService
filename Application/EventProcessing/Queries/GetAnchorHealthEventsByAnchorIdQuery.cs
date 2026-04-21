using Application.Common.Models;
using Application.DTOs.EventProcessing;
using Domain.Abstractions;
using MediatR;

namespace Application.EventProcessing.Queries;

public sealed record GetAnchorHealthEventsByAnchorIdQuery(
    Guid AnchorId,
    int Page = 1,
    int PageSize = 100
) : IRequest<Result<IReadOnlyList<AnchorHealthEventDto>>>;

public sealed class GetAnchorHealthEventsByAnchorIdQueryHandler : IRequestHandler<GetAnchorHealthEventsByAnchorIdQuery, Result<IReadOnlyList<AnchorHealthEventDto>>>
{
    private readonly IAnchorHealthEventRepository _anchorHealthEventRepository;

    public GetAnchorHealthEventsByAnchorIdQueryHandler(IAnchorHealthEventRepository anchorHealthEventRepository)
    {
        _anchorHealthEventRepository = anchorHealthEventRepository;
    }

    public async Task<Result<IReadOnlyList<AnchorHealthEventDto>>> Handle(GetAnchorHealthEventsByAnchorIdQuery request, CancellationToken ct)
    {
        var (items, _) = await _anchorHealthEventRepository.GetPagedByAnchorIdAsync(
            request.AnchorId, request.Page, request.PageSize, ct);

        var dto = items.Select(x => new AnchorHealthEventDto(
            x.Id,
            x.RawEventId,
            x.AnchorId,
            x.EventTimestamp,
            x.Uptime,
            x.Temperature,
            x.CpuUsage,
            x.MemoryUsage,
            x.TagCount,
            x.PacketLossRate)).ToList();

        return Result<IReadOnlyList<AnchorHealthEventDto>>.Success(dto);
    }
}