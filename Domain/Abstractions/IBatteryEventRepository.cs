using Domain.Entities;

namespace Domain.Abstractions;

public interface IBatteryEventRepository
{
    Task<BatteryEvent?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(BatteryEvent batteryEvent, CancellationToken ct = default);

    Task<(IReadOnlyList<BatteryEvent> Items, int Total)> GetPagedByTagIdAsync(
        Guid tagId,
        int page,
        int pageSize,
        CancellationToken ct = default);

    IQueryable<BatteryEvent> Query();
}