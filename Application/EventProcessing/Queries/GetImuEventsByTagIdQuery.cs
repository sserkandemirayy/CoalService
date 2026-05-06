using Application.Common.Models;
using Application.DTOs.EventProcessing;
using Domain.Abstractions;
using MediatR;

namespace Application.EventProcessing.Queries;

public sealed record GetImuEventsByTagIdQuery(
    Guid TagId,
    int Page = 1,
    int PageSize = 100
) : IRequest<Result<IReadOnlyList<ImuEventDto>>>;

public sealed class GetImuEventsByTagIdQueryHandler : IRequestHandler<GetImuEventsByTagIdQuery, Result<IReadOnlyList<ImuEventDto>>>
{
    private readonly IImuEventRepository _imuEventRepository;

    public GetImuEventsByTagIdQueryHandler(IImuEventRepository imuEventRepository)
    {
        _imuEventRepository = imuEventRepository;
    }

    public async Task<Result<IReadOnlyList<ImuEventDto>>> Handle(GetImuEventsByTagIdQuery request, CancellationToken ct)
    {
        var (items, _) = await _imuEventRepository.GetPagedByTagIdAsync(
            request.TagId,
            request.Page,
            request.PageSize,
            ct);

        var dto = items
            .Select(x => new ImuEventDto(
                x.Id,
                x.RawEventId,
                x.TagId,
                x.EventTimestamp,
                x.EventType.ToString()))
            .ToList();

        return Result<IReadOnlyList<ImuEventDto>>.Success(dto);
    }
}