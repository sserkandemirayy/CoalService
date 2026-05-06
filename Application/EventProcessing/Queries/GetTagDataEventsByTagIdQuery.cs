using Application.Common.Models;
using Application.DTOs.EventProcessing;
using Domain.Abstractions;
using MediatR;

namespace Application.EventProcessing.Queries;

public sealed record GetTagDataEventsByTagIdQuery(
    Guid TagId,
    int Page = 1,
    int PageSize = 100
) : IRequest<Result<IReadOnlyList<TagDataEventDto>>>;

public sealed class GetTagDataEventsByTagIdQueryHandler : IRequestHandler<GetTagDataEventsByTagIdQuery, Result<IReadOnlyList<TagDataEventDto>>>
{
    private readonly ITagDataEventRepository _tagDataEventRepository;

    public GetTagDataEventsByTagIdQueryHandler(ITagDataEventRepository tagDataEventRepository)
    {
        _tagDataEventRepository = tagDataEventRepository;
    }

    public async Task<Result<IReadOnlyList<TagDataEventDto>>> Handle(GetTagDataEventsByTagIdQuery request, CancellationToken ct)
    {
        var (items, _) = await _tagDataEventRepository.GetPagedByTagIdAsync(
            request.TagId, request.Page, request.PageSize, ct);

        var dto = items.Select(x => new TagDataEventDto(
            x.Id,
            x.RawEventId,
            x.AnchorId,
            x.TagId,
            x.EventTimestamp,
            x.ReportedTagType)).ToList();

        return Result<IReadOnlyList<TagDataEventDto>>.Success(dto);
    }
}