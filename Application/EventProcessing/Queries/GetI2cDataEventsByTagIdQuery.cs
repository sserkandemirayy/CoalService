using Application.Common.Models;
using Application.DTOs.EventProcessing;
using Domain.Abstractions;
using MediatR;

namespace Application.EventProcessing.Queries;

public sealed record GetI2cDataEventsByTagIdQuery(
    Guid TagId,
    int Page = 1,
    int PageSize = 100
) : IRequest<Result<IReadOnlyList<I2cDataEventDto>>>;

public sealed class GetI2cDataEventsByTagIdQueryHandler : IRequestHandler<GetI2cDataEventsByTagIdQuery, Result<IReadOnlyList<I2cDataEventDto>>>
{
    private readonly II2cDataEventRepository _i2cDataEventRepository;

    public GetI2cDataEventsByTagIdQueryHandler(II2cDataEventRepository i2cDataEventRepository)
    {
        _i2cDataEventRepository = i2cDataEventRepository;
    }

    public async Task<Result<IReadOnlyList<I2cDataEventDto>>> Handle(GetI2cDataEventsByTagIdQuery request, CancellationToken ct)
    {
        var (items, _) = await _i2cDataEventRepository.GetPagedByTagIdAsync(
            request.TagId, request.Page, request.PageSize, ct);

        var dto = items.Select(x => new I2cDataEventDto(
            x.Id,
            x.RawEventId,
            x.TagId,
            x.EventTimestamp,
            x.Address,
            x.Register,
            x.Direction.ToString(),
            x.Ack,
            x.DataJson)).ToList();

        return Result<IReadOnlyList<I2cDataEventDto>>.Success(dto);
    }
}