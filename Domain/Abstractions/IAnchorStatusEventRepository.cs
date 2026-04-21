using Domain.Entities;

namespace Domain.Abstractions;

public interface IAnchorStatusEventRepository
{
    Task<AnchorStatusEvent?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(AnchorStatusEvent statusEvent, CancellationToken ct = default);

    Task<(IReadOnlyList<AnchorStatusEvent> Items, int Total)> GetPagedByAnchorIdAsync(
        Guid anchorId,
        int page,
        int pageSize,
        CancellationToken ct = default);

    IQueryable<AnchorStatusEvent> Query();
}