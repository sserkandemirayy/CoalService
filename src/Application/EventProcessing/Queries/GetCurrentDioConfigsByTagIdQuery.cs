using Application.Common.Models;
using Application.DTOs.EventProcessing;
using Domain.Abstractions;
using MediatR;

namespace Application.EventProcessing.Queries;

public sealed record GetCurrentDioConfigsByTagIdQuery(Guid TagId)
    : IRequest<Result<IReadOnlyList<TagDioConfigSnapshotDto>>>;

public sealed class GetCurrentDioConfigsByTagIdQueryHandler
    : IRequestHandler<GetCurrentDioConfigsByTagIdQuery, Result<IReadOnlyList<TagDioConfigSnapshotDto>>>
{
    private readonly ITagDioConfigSnapshotRepository _repo;

    public GetCurrentDioConfigsByTagIdQueryHandler(ITagDioConfigSnapshotRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<IReadOnlyList<TagDioConfigSnapshotDto>>> Handle(GetCurrentDioConfigsByTagIdQuery request, CancellationToken ct)
    {
        var items = await _repo.GetByTagIdAsync(request.TagId, ct);

        var dto = items.Select(x => new TagDioConfigSnapshotDto(
            x.Id, x.TagId, x.Pin, x.LastRawEventId, x.LastReportedAt,
            x.Direction.ToString(), x.PeriodicReportEnabled, x.PeriodicReportInterval,
            x.ReportOnChange, x.LowPassFilterJson)).ToList();

        return Result<IReadOnlyList<TagDioConfigSnapshotDto>>.Success(dto);
    }
}