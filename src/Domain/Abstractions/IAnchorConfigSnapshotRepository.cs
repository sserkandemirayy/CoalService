using Domain.Entities;

namespace Domain.Abstractions;

public interface IAnchorConfigSnapshotRepository
{
    Task<AnchorConfigSnapshot?> GetByAnchorIdAsync(Guid anchorId, CancellationToken ct = default);
    Task AddAsync(AnchorConfigSnapshot entity, CancellationToken ct = default);
    Task UpdateAsync(AnchorConfigSnapshot entity, CancellationToken ct = default);
    IQueryable<AnchorConfigSnapshot> Query();
}