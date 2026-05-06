using Domain.Entities;

namespace Domain.Abstractions;

public interface IAnchorHealthEventRepository
{
    Task<AnchorHealthEvent?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(AnchorHealthEvent healthEvent, CancellationToken ct = default);

    Task<(IReadOnlyList<AnchorHealthEvent> Items, int Total)> GetPagedByAnchorIdAsync(
        Guid anchorId,
        int page,
        int pageSize,
        CancellationToken ct = default);

    IQueryable<AnchorHealthEvent> Query();
}