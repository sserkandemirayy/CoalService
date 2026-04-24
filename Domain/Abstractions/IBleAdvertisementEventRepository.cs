using Domain.Entities;

namespace Domain.Abstractions;

public interface IBleAdvertisementEventRepository
{
    Task<BleAdvertisementEvent?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(BleAdvertisementEvent entity, CancellationToken ct = default);

    Task<(IReadOnlyList<BleAdvertisementEvent> Items, int Total)> GetPagedByTagIdAsync(
        Guid tagId,
        int page,
        int pageSize,
        CancellationToken ct = default);

    Task<(IReadOnlyList<BleAdvertisementEvent> Items, int Total)> GetPagedByAnchorIdAsync(
        Guid anchorId,
        int page,
        int pageSize,
        CancellationToken ct = default);

    IQueryable<BleAdvertisementEvent> Query();
}