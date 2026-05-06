using Application.Common.Models;
using Application.DTOs.EventProcessing;
using Domain.Abstractions;
using MediatR;

namespace Application.EventProcessing.Queries;

public sealed record GetBatteryEventsByTagIdQuery(
    Guid TagId,
    int Page = 1,
    int PageSize = 100
) : IRequest<Result<IReadOnlyList<BatteryEventDto>>>;

public sealed class GetBatteryEventsByTagIdQueryHandler : IRequestHandler<GetBatteryEventsByTagIdQuery, Result<IReadOnlyList<BatteryEventDto>>>
{
    private readonly IBatteryEventRepository _batteryEventRepository;

    public GetBatteryEventsByTagIdQueryHandler(IBatteryEventRepository batteryEventRepository)
    {
        _batteryEventRepository = batteryEventRepository;
    }

    public async Task<Result<IReadOnlyList<BatteryEventDto>>> Handle(GetBatteryEventsByTagIdQuery request, CancellationToken ct)
    {
        var (items, _) = await _batteryEventRepository.GetPagedByTagIdAsync(
            request.TagId,
            request.Page,
            request.PageSize,
            ct);

        var dto = items
            .Select(x => new BatteryEventDto(
                x.Id,
                x.RawEventId,
                x.TagId,
                x.AnchorId,
                x.EventTimestamp,
                x.BatteryLevel))
            .ToList();

        return Result<IReadOnlyList<BatteryEventDto>>.Success(dto);
    }
}