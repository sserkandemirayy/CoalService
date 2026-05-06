using Domain.Entities;

namespace Domain.Abstractions;

public interface IBleConfigEventRepository
{
    Task<BleConfigEvent?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(BleConfigEvent entity, CancellationToken ct = default);

    Task<(IReadOnlyList<BleConfigEvent> Items, int Total)> GetPagedByTagIdAsync(
        Guid tagId,
        int page,
        int pageSize,
        CancellationToken ct = default);

    IQueryable<BleConfigEvent> Query();
}