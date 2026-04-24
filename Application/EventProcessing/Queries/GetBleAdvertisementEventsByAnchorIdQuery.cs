using Application.Common.Models;
using Application.DTOs.EventProcessing;
using Domain.Abstractions;
using MediatR;

namespace Application.EventProcessing.Queries;

public sealed record GetBleAdvertisementEventsByAnchorIdQuery(
    Guid AnchorId,
    int Page = 1,
    int PageSize = 100
) : IRequest<Result<IReadOnlyList<BleAdvertisementEventDto>>>;

public sealed class GetBleAdvertisementEventsByAnchorIdQueryHandler : IRequestHandler<GetBleAdvertisementEventsByAnchorIdQuery, Result<IReadOnlyList<BleAdvertisementEventDto>>>
{
    private readonly IBleAdvertisementEventRepository _bleAdvertisementEventRepository;

    public GetBleAdvertisementEventsByAnchorIdQueryHandler(IBleAdvertisementEventRepository bleAdvertisementEventRepository)
    {
        _bleAdvertisementEventRepository = bleAdvertisementEventRepository;
    }

    public async Task<Result<IReadOnlyList<BleAdvertisementEventDto>>> Handle(GetBleAdvertisementEventsByAnchorIdQuery request, CancellationToken ct)
    {
        var (items, _) = await _bleAdvertisementEventRepository.GetPagedByAnchorIdAsync(
            request.AnchorId, request.Page, request.PageSize, ct);

        var dto = items.Select(x => new BleAdvertisementEventDto(
            x.Id,
            x.RawEventId,
            x.AnchorId,
            x.TagId,
            x.EventTimestamp,
            x.Rssi)).ToList();

        return Result<IReadOnlyList<BleAdvertisementEventDto>>.Success(dto);
    }
}