using Application.Common.Models;
using Application.DTOs.EventProcessing;
using Domain.Abstractions;
using MediatR;

namespace Application.EventProcessing.Queries;

public sealed record GetUwbTagToTagRangingEventsByTagIdQuery(
    Guid TagId,
    int Page = 1,
    int PageSize = 100
) : IRequest<Result<IReadOnlyList<UwbTagToTagRangingEventDto>>>;

public sealed class GetUwbTagToTagRangingEventsByTagIdQueryHandler : IRequestHandler<GetUwbTagToTagRangingEventsByTagIdQuery, Result<IReadOnlyList<UwbTagToTagRangingEventDto>>>
{
    private readonly IUwbTagToTagRangingEventRepository _uwbTagToTagRangingEventRepository;

    public GetUwbTagToTagRangingEventsByTagIdQueryHandler(IUwbTagToTagRangingEventRepository uwbTagToTagRangingEventRepository)
    {
        _uwbTagToTagRangingEventRepository = uwbTagToTagRangingEventRepository;
    }

    public async Task<Result<IReadOnlyList<UwbTagToTagRangingEventDto>>> Handle(GetUwbTagToTagRangingEventsByTagIdQuery request, CancellationToken ct)
    {
        var (items, _) = await _uwbTagToTagRangingEventRepository.GetPagedByTagIdAsync(
            request.TagId, request.Page, request.PageSize, ct);

        var dto = items.Select(x => new UwbTagToTagRangingEventDto(
            x.Id,
            x.RawEventId,
            x.TagId,
            x.PeerTagId,
            x.EventTimestamp,
            x.Distance)).ToList();

        return Result<IReadOnlyList<UwbTagToTagRangingEventDto>>.Success(dto);
    }
}