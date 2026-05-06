using Application.Common.Models;
using Application.DTOs.EventProcessing;
using Domain.Abstractions;
using MediatR;

namespace Application.EventProcessing.Queries;

public sealed record GetBleConfigEventsByTagIdQuery(Guid TagId, int Page = 1, int PageSize = 100)
    : IRequest<Result<IReadOnlyList<BleConfigEventDto>>>;

public sealed class GetBleConfigEventsByTagIdQueryHandler
    : IRequestHandler<GetBleConfigEventsByTagIdQuery, Result<IReadOnlyList<BleConfigEventDto>>>
{
    private readonly IBleConfigEventRepository _repo;

    public GetBleConfigEventsByTagIdQueryHandler(IBleConfigEventRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<IReadOnlyList<BleConfigEventDto>>> Handle(GetBleConfigEventsByTagIdQuery request, CancellationToken ct)
    {
        var (items, _) = await _repo.GetPagedByTagIdAsync(request.TagId, request.Page, request.PageSize, ct);

        var dto = items.Select(x => new BleConfigEventDto(
            x.Id, x.RawEventId, x.TagId, x.EventTimestamp, x.Enabled,
            x.TxPower, x.AdvertisementInterval, x.MeshEnabled)).ToList();

        return Result<IReadOnlyList<BleConfigEventDto>>.Success(dto);
    }
}