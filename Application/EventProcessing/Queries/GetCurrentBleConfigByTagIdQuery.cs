using Application.Common.Models;
using Application.DTOs.EventProcessing;
using Domain.Abstractions;
using MediatR;

namespace Application.EventProcessing.Queries;

public sealed record GetCurrentBleConfigByTagIdQuery(Guid TagId)
    : IRequest<Result<TagBleConfigSnapshotDto>>;

public sealed class GetCurrentBleConfigByTagIdQueryHandler
    : IRequestHandler<GetCurrentBleConfigByTagIdQuery, Result<TagBleConfigSnapshotDto>>
{
    private readonly ITagBleConfigSnapshotRepository _repo;

    public GetCurrentBleConfigByTagIdQueryHandler(ITagBleConfigSnapshotRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<TagBleConfigSnapshotDto>> Handle(GetCurrentBleConfigByTagIdQuery request, CancellationToken ct)
    {
        var x = await _repo.GetByTagIdAsync(request.TagId, ct);
        if (x is null)
            return Result<TagBleConfigSnapshotDto>.Failure("BLE config snapshot not found.");

        return Result<TagBleConfigSnapshotDto>.Success(new TagBleConfigSnapshotDto(
            x.Id, x.TagId, x.LastRawEventId, x.LastReportedAt,
            x.Enabled, x.TxPower, x.AdvertisementInterval, x.MeshEnabled));
    }
}