using Domain.Entities;

namespace Domain.Abstractions;

public interface IUwbRangingEventRepository
{
    Task<UwbRangingEvent?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(UwbRangingEvent uwbRangingEvent, CancellationToken ct = default);

    Task<(IReadOnlyList<UwbRangingEvent> Items, int Total)> GetPagedByTagIdAsync(
        Guid tagId,
        int page,
        int pageSize,
        CancellationToken ct = default);

    Task<(IReadOnlyList<UwbRangingEvent> Items, int Total)> GetPagedByAnchorIdAsync(
        Guid anchorId,
        int page,
        int pageSize,
        CancellationToken ct = default);

    IQueryable<UwbRangingEvent> Query();
}