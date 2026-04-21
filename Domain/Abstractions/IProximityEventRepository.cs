using Domain.Entities;

namespace Domain.Abstractions;

public interface IProximityEventRepository
{
    Task<ProximityEvent?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(ProximityEvent proximityEvent, CancellationToken ct = default);

    Task<(IReadOnlyList<ProximityEvent> Items, int Total)> GetPagedByTagIdAsync(
        Guid tagId,
        int page,
        int pageSize,
        CancellationToken ct = default);

    IQueryable<ProximityEvent> Query();
}