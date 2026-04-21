using Application.Common.Models;
using Application.DTOs.EventProcessing;
using Domain.Abstractions;
using MediatR;

namespace Application.EventProcessing.Queries;

public sealed record GetAnchorStatusEventsByAnchorIdQuery(
    Guid AnchorId,
    int Page = 1,
    int PageSize = 100
) : IRequest<Result<IReadOnlyList<AnchorStatusEventDto>>>;

public sealed class GetAnchorStatusEventsByAnchorIdQueryHandler : IRequestHandler<GetAnchorStatusEventsByAnchorIdQuery, Result<IReadOnlyList<AnchorStatusEventDto>>>
{
    private readonly IAnchorStatusEventRepository _anchorStatusEventRepository;

    public GetAnchorStatusEventsByAnchorIdQueryHandler(IAnchorStatusEventRepository anchorStatusEventRepository)
    {
        _anchorStatusEventRepository = anchorStatusEventRepository;
    }

    public async Task<Result<IReadOnlyList<AnchorStatusEventDto>>> Handle(GetAnchorStatusEventsByAnchorIdQuery request, CancellationToken ct)
    {
        var (items, _) = await _anchorStatusEventRepository.GetPagedByAnchorIdAsync(
            request.AnchorId,
            request.Page,
            request.PageSize,
            ct);

        var dto = items
            .Select(x => new AnchorStatusEventDto(
                x.Id,
                x.RawEventId,
                x.AnchorId,
                x.EventTimestamp,
                x.Status.ToString(),
                x.PreviousStatus.ToString(),
                x.Reason))
            .ToList();

        return Result<IReadOnlyList<AnchorStatusEventDto>>.Success(dto);
    }
}