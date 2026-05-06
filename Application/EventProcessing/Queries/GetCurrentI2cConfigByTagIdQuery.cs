using Application.Common.Models;
using Application.DTOs.EventProcessing;
using Domain.Abstractions;
using MediatR;

namespace Application.EventProcessing.Queries;

public sealed record GetCurrentI2cConfigByTagIdQuery(Guid TagId)
    : IRequest<Result<TagI2cConfigSnapshotDto>>;

public sealed class GetCurrentI2cConfigByTagIdQueryHandler
    : IRequestHandler<GetCurrentI2cConfigByTagIdQuery, Result<TagI2cConfigSnapshotDto>>
{
    private readonly ITagI2cConfigSnapshotRepository _repo;

    public GetCurrentI2cConfigByTagIdQueryHandler(ITagI2cConfigSnapshotRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<TagI2cConfigSnapshotDto>> Handle(GetCurrentI2cConfigByTagIdQuery request, CancellationToken ct)
    {
        var x = await _repo.GetByTagIdAsync(request.TagId, ct);
        if (x is null)
            return Result<TagI2cConfigSnapshotDto>.Failure("I2C config snapshot not found.");

        return Result<TagI2cConfigSnapshotDto>.Success(new TagI2cConfigSnapshotDto(
            x.Id, x.TagId, x.LastRawEventId, x.LastReportedAt,
            x.Enabled, x.ClockSpeed, x.DevicesJson));
    }
}