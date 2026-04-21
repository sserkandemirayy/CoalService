using Domain.Entities;

namespace Domain.Abstractions;

public interface II2cConfigEventRepository
{
    Task<I2cConfigEvent?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(I2cConfigEvent entity, CancellationToken ct = default);

    Task<(IReadOnlyList<I2cConfigEvent> Items, int Total)> GetPagedByTagIdAsync(
        Guid tagId,
        int page,
        int pageSize,
        CancellationToken ct = default);

    IQueryable<I2cConfigEvent> Query();
}