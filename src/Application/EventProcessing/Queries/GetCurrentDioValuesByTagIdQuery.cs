using Application.Common.Models;
using Application.DTOs.EventProcessing;
using Domain.Abstractions;
using MediatR;

namespace Application.EventProcessing.Queries;

public sealed record GetCurrentDioValuesByTagIdQuery(Guid TagId)
    : IRequest<Result<IReadOnlyList<TagDioValueSnapshotDto>>>;

public sealed class GetCurrentDioValuesByTagIdQueryHandler : IRequestHandler<GetCurrentDioValuesByTagIdQuery, Result<IReadOnlyList<TagDioValueSnapshotDto>>>
{
    private readonly ITagDioValueSnapshotRepository _tagDioValueSnapshotRepository;

    public GetCurrentDioValuesByTagIdQueryHandler(ITagDioValueSnapshotRepository tagDioValueSnapshotRepository)
    {
        _tagDioValueSnapshotRepository = tagDioValueSnapshotRepository;
    }

    public async Task<Result<IReadOnlyList<TagDioValueSnapshotDto>>> Handle(GetCurrentDioValuesByTagIdQuery request, CancellationToken ct)
    {
        var items = await _tagDioValueSnapshotRepository.GetByTagIdAsync(request.TagId, ct);

        var dto = items.Select(x => new TagDioValueSnapshotDto(
            x.Id,
            x.TagId,
            x.Pin,
            x.LastRawEventId,
            x.LastReportedAt,
            x.Value)).ToList();

        return Result<IReadOnlyList<TagDioValueSnapshotDto>>.Success(dto);
    }
}