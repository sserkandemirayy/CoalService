using Application.Common.Models;
using Application.DTOs.EventProcessing;
using Domain.Abstractions;
using MediatR;

namespace Application.EventProcessing.Queries;

public sealed record GetUwbConfigEventsByTagIdQuery(Guid TagId, int Page = 1, int PageSize = 100)
    : IRequest<Result<IReadOnlyList<UwbConfigEventDto>>>;

public sealed class GetUwbConfigEventsByTagIdQueryHandler
    : IRequestHandler<GetUwbConfigEventsByTagIdQuery, Result<IReadOnlyList<UwbConfigEventDto>>>
{
    private readonly IUwbConfigEventRepository _repo;

    public GetUwbConfigEventsByTagIdQueryHandler(IUwbConfigEventRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<IReadOnlyList<UwbConfigEventDto>>> Handle(GetUwbConfigEventsByTagIdQuery request, CancellationToken ct)
    {
        var (items, _) = await _repo.GetPagedByTagIdAsync(request.TagId, request.Page, request.PageSize, ct);

        var dto = items.Select(x => new UwbConfigEventDto(
            x.Id, x.RawEventId, x.TagId, x.EventTimestamp,
            x.Enabled, x.Channel, x.TxPower, x.RangingInterval,
            x.TagToTagEnabled, x.TagToTagInterval)).ToList();

        return Result<IReadOnlyList<UwbConfigEventDto>>.Success(dto);
    }
}