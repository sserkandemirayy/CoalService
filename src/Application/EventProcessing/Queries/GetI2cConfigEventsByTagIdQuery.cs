using Application.Common.Models;
using Application.DTOs.EventProcessing;
using Domain.Abstractions;
using MediatR;

namespace Application.EventProcessing.Queries;

public sealed record GetI2cConfigEventsByTagIdQuery(Guid TagId, int Page = 1, int PageSize = 100)
    : IRequest<Result<IReadOnlyList<I2cConfigEventDto>>>;

public sealed class GetI2cConfigEventsByTagIdQueryHandler
    : IRequestHandler<GetI2cConfigEventsByTagIdQuery, Result<IReadOnlyList<I2cConfigEventDto>>>
{
    private readonly II2cConfigEventRepository _repo;

    public GetI2cConfigEventsByTagIdQueryHandler(II2cConfigEventRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<IReadOnlyList<I2cConfigEventDto>>> Handle(GetI2cConfigEventsByTagIdQuery request, CancellationToken ct)
    {
        var (items, _) = await _repo.GetPagedByTagIdAsync(request.TagId, request.Page, request.PageSize, ct);

        var dto = items.Select(x => new I2cConfigEventDto(
            x.Id, x.RawEventId, x.TagId, x.EventTimestamp, x.Enabled, x.ClockSpeed, x.DevicesJson)).ToList();

        return Result<IReadOnlyList<I2cConfigEventDto>>.Success(dto);
    }
}