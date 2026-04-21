using Application.Common.Models;
using Application.DTOs.EventProcessing;
using Domain.Abstractions;
using MediatR;

namespace Application.EventProcessing.Queries;

public sealed record GetProximityEventsByTagIdQuery(
    Guid TagId,
    int Page = 1,
    int PageSize = 100
) : IRequest<Result<IReadOnlyList<ProximityEventDto>>>;

public sealed class GetProximityEventsByTagIdQueryHandler : IRequestHandler<GetProximityEventsByTagIdQuery, Result<IReadOnlyList<ProximityEventDto>>>
{
    private readonly IProximityEventRepository _proximityEventRepository;

    public GetProximityEventsByTagIdQueryHandler(IProximityEventRepository proximityEventRepository)
    {
        _proximityEventRepository = proximityEventRepository;
    }

    public async Task<Result<IReadOnlyList<ProximityEventDto>>> Handle(GetProximityEventsByTagIdQuery request, CancellationToken ct)
    {
        var (items, _) = await _proximityEventRepository.GetPagedByTagIdAsync(
            request.TagId,
            request.Page,
            request.PageSize,
            ct);

        var dto = items
            .Select(x => new ProximityEventDto(
                x.Id,
                x.RawEventId,
                x.TagId,
                x.PeerTagId,
                x.EventTimestamp,
                x.Distance,
                x.Threshold,
                x.Severity.ToString()))
            .ToList();

        return Result<IReadOnlyList<ProximityEventDto>>.Success(dto);
    }
}