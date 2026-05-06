using Domain.Entities;

namespace Domain.Abstractions;

public interface IAnchorHeartbeatEventRepository
{
    Task<AnchorHeartbeatEvent?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(AnchorHeartbeatEvent heartbeatEvent, CancellationToken ct = default);

    Task<(IReadOnlyList<AnchorHeartbeatEvent> Items, int Total)> GetPagedByAnchorIdAsync(
        Guid anchorId,
        int page,
        int pageSize,
        CancellationToken ct = default);

    IQueryable<AnchorHeartbeatEvent> Query();
}