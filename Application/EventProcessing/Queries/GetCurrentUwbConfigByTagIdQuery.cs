using Application.Common.Models;
using Application.DTOs.EventProcessing;
using Domain.Abstractions;
using MediatR;

namespace Application.EventProcessing.Queries;

public sealed record GetCurrentUwbConfigByTagIdQuery(Guid TagId)
    : IRequest<Result<TagUwbConfigSnapshotDto>>;

public sealed class GetCurrentUwbConfigByTagIdQueryHandler
    : IRequestHandler<GetCurrentUwbConfigByTagIdQuery, Result<TagUwbConfigSnapshotDto>>
{
    private readonly ITagUwbConfigSnapshotRepository _repo;

    public GetCurrentUwbConfigByTagIdQueryHandler(ITagUwbConfigSnapshotRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<TagUwbConfigSnapshotDto>> Handle(GetCurrentUwbConfigByTagIdQuery request, CancellationToken ct)
    {
        var x = await _repo.GetByTagIdAsync(request.TagId, ct);
        if (x is null)
            return Result<TagUwbConfigSnapshotDto>.Failure("UWB config snapshot not found.");

        return Result<TagUwbConfigSnapshotDto>.Success(new TagUwbConfigSnapshotDto(
            x.Id, x.TagId, x.LastRawEventId, x.LastReportedAt,
            x.Enabled, x.Channel, x.TxPower, x.RangingInterval,
            x.TagToTagEnabled, x.TagToTagInterval));
    }
}