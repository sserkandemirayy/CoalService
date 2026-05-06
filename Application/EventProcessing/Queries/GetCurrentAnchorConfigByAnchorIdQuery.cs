using Application.Common.Models;
using Application.DTOs.EventProcessing;
using Domain.Abstractions;
using MediatR;

namespace Application.EventProcessing.Queries;

public sealed record GetCurrentAnchorConfigByAnchorIdQuery(Guid AnchorId)
    : IRequest<Result<AnchorConfigSnapshotDto>>;

public sealed class GetCurrentAnchorConfigByAnchorIdQueryHandler
    : IRequestHandler<GetCurrentAnchorConfigByAnchorIdQuery, Result<AnchorConfigSnapshotDto>>
{
    private readonly IAnchorConfigSnapshotRepository _repo;

    public GetCurrentAnchorConfigByAnchorIdQueryHandler(IAnchorConfigSnapshotRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<AnchorConfigSnapshotDto>> Handle(GetCurrentAnchorConfigByAnchorIdQuery request, CancellationToken ct)
    {
        var x = await _repo.GetByAnchorIdAsync(request.AnchorId, ct);
        if (x is null)
            return Result<AnchorConfigSnapshotDto>.Failure("Anchor config snapshot not found.");

        return Result<AnchorConfigSnapshotDto>.Success(new AnchorConfigSnapshotDto(
            x.Id, x.AnchorId, x.LastRawEventId, x.LastReportedAt, x.FirmwareVersion,
            x.PositionJson, x.NetworkJson, x.UwbJson, x.BleJson,
            x.HeartbeatInterval, x.ReportInterval));
    }
}