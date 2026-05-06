using Application.Common.Models;
using Application.DTOs.EventProcessing;
using Domain.Abstractions;
using MediatR;

namespace Application.EventProcessing.Queries;

public sealed record GetLocationEventsByTagIdQuery(
    Guid TagId,
    int Page = 1,
    int PageSize = 100
) : IRequest<Result<IReadOnlyList<LocationEventDto>>>;

public sealed class GetLocationEventsByTagIdQueryHandler : IRequestHandler<GetLocationEventsByTagIdQuery, Result<IReadOnlyList<LocationEventDto>>>
{
    private readonly ILocationEventRepository _locationEventRepository;

    public GetLocationEventsByTagIdQueryHandler(ILocationEventRepository locationEventRepository)
    {
        _locationEventRepository = locationEventRepository;
    }

    public async Task<Result<IReadOnlyList<LocationEventDto>>> Handle(GetLocationEventsByTagIdQuery request, CancellationToken ct)
    {
        var (items, _) = await _locationEventRepository.GetPagedByTagIdAsync(
            request.TagId,
            request.Page,
            request.PageSize,
            ct);

        var dto = items
            .Select(x => new LocationEventDto(
                x.Id,
                x.RawEventId,
                x.TagId,
                x.EventTimestamp,
                x.X,
                x.Y,
                x.Z,
                x.Accuracy,
                x.Confidence,
                x.UsedAnchorsJson))
            .ToList();

        return Result<IReadOnlyList<LocationEventDto>>.Success(dto);
    }
}