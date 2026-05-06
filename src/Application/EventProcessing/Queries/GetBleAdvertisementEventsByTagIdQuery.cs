using Application.Common.Models;
using Application.DTOs.EventProcessing;
using Domain.Abstractions;
using MediatR;

namespace Application.EventProcessing.Queries;

public sealed record GetBleAdvertisementEventsByTagIdQuery(
    Guid TagId,
    int Page = 1,
    int PageSize = 100
) : IRequest<Result<IReadOnlyList<BleAdvertisementEventDto>>>;

public sealed class GetBleAdvertisementEventsByTagIdQueryHandler : IRequestHandler<GetBleAdvertisementEventsByTagIdQuery, Result<IReadOnlyList<BleAdvertisementEventDto>>>
{
    private readonly IBleAdvertisementEventRepository _bleAdvertisementEventRepository;

    public GetBleAdvertisementEventsByTagIdQueryHandler(IBleAdvertisementEventRepository bleAdvertisementEventRepository)
    {
        _bleAdvertisementEventRepository = bleAdvertisementEventRepository;
    }

    public async Task<Result<IReadOnlyList<BleAdvertisementEventDto>>> Handle(GetBleAdvertisementEventsByTagIdQuery request, CancellationToken ct)
    {
        var (items, _) = await _bleAdvertisementEventRepository.GetPagedByTagIdAsync(
            request.TagId, request.Page, request.PageSize, ct);

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