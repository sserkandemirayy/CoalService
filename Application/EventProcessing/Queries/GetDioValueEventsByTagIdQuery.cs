using Application.Common.Models;
using Application.DTOs.EventProcessing;
using Domain.Abstractions;
using MediatR;

namespace Application.EventProcessing.Queries;

public sealed record GetDioValueEventsByTagIdQuery(
    Guid TagId,
    int Page = 1,
    int PageSize = 100
) : IRequest<Result<IReadOnlyList<DioValueEventDto>>>;

public sealed class GetDioValueEventsByTagIdQueryHandler : IRequestHandler<GetDioValueEventsByTagIdQuery, Result<IReadOnlyList<DioValueEventDto>>>
{
    private readonly IDioValueEventRepository _dioValueEventRepository;

    public GetDioValueEventsByTagIdQueryHandler(IDioValueEventRepository dioValueEventRepository)
    {
        _dioValueEventRepository = dioValueEventRepository;
    }

    public async Task<Result<IReadOnlyList<DioValueEventDto>>> Handle(GetDioValueEventsByTagIdQuery request, CancellationToken ct)
    {
        var (items, _) = await _dioValueEventRepository.GetPagedByTagIdAsync(
            request.TagId, request.Page, request.PageSize, ct);

        var dto = items.Select(x => new DioValueEventDto(
            x.Id,
            x.RawEventId,
            x.TagId,
            x.EventTimestamp,
            x.Pin,
            x.Value)).ToList();

        return Result<IReadOnlyList<DioValueEventDto>>.Success(dto);
    }
}