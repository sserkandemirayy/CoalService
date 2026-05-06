using Domain.Entities;

namespace Domain.Abstractions;

public interface IAnchorConfigEventRepository
{
    Task<AnchorConfigEvent?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(AnchorConfigEvent entity, CancellationToken ct = default);

    Task<(IReadOnlyList<AnchorConfigEvent> Items, int Total)> GetPagedByAnchorIdAsync(
        Guid anchorId,
        int page,
        int pageSize,
        CancellationToken ct = default);

    IQueryable<AnchorConfigEvent> Query();
}