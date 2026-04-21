using Application.Common.Models;
using Application.DTOs.EventProcessing;
using Domain.Abstractions;
using MediatR;

namespace Application.EventProcessing.Queries;

public sealed record GetUwbRangingEventsByTagIdQuery(
    Guid TagId,
    int Page = 1,
    int PageSize = 100
) : IRequest<Result<IReadOnlyList<UwbRangingEventDto>>>;

public sealed class GetUwbRangingEventsByTagIdQueryHandler : IRequestHandler<GetUwbRangingEventsByTagIdQuery, Result<IReadOnlyList<UwbRangingEventDto>>>
{
    private readonly IUwbRangingEventRepository _uwbRangingEventRepository;

    public GetUwbRangingEventsByTagIdQueryHandler(IUwbRangingEventRepository uwbRangingEventRepository)
    {
        _uwbRangingEventRepository = uwbRangingEventRepository;
    }

    public async Task<Result<IReadOnlyList<UwbRangingEventDto>>> Handle(GetUwbRangingEventsByTagIdQuery request, CancellationToken ct)
    {
        var (items, _) = await _uwbRangingEventRepository.GetPagedByTagIdAsync(
            request.TagId, request.Page, request.PageSize, ct);

        var dto = items.Select(x => new UwbRangingEventDto(
            x.Id,
            x.RawEventId,
            x.AnchorId,
            x.TagId,
            x.EventTimestamp,
            x.Distance)).ToList();

        return Result<IReadOnlyList<UwbRangingEventDto>>.Success(dto);
    }
}