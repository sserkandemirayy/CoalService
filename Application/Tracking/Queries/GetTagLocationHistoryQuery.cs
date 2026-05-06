using Application.Common.Models;
using Application.DTOs.Tracking;
using Domain.Abstractions;
using MediatR;

namespace Application.Tracking.Queries;

public sealed record GetTagLocationHistoryQuery(
    Guid TagId,
    DateTime? From,
    DateTime? To,
    int Page = 1,
    int PageSize = 500
) : IRequest<Result<IReadOnlyList<TagHistoryPointDto>>>;

public sealed class GetTagLocationHistoryQueryHandler : IRequestHandler<GetTagLocationHistoryQuery, Result<IReadOnlyList<TagHistoryPointDto>>>
{
    private readonly ILocationEventRepository _locationEventRepository;

    public GetTagLocationHistoryQueryHandler(ILocationEventRepository locationEventRepository)
    {
        _locationEventRepository = locationEventRepository;
    }

    public async Task<Result<IReadOnlyList<TagHistoryPointDto>>> Handle(GetTagLocationHistoryQuery request, CancellationToken ct)
    {
        var (items, _) = await _locationEventRepository.GetPagedHistoryAsync(
            request.TagId,
            request.From,
            request.To,
            request.Page,
            request.PageSize,
            ct);

        var dto = items
            .Select(x => new TagHistoryPointDto(
                x.Id,
                x.EventTimestamp,
                x.X,
                x.Y,
                x.Z,
                x.Accuracy,
                x.Confidence))
            .ToList();

        return Result<IReadOnlyList<TagHistoryPointDto>>.Success(dto);
    }
}