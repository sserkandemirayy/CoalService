using Domain.Entities;

namespace Domain.Abstractions;

public interface II2cDataEventRepository
{
    Task<I2cDataEvent?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(I2cDataEvent entity, CancellationToken ct = default);

    Task<(IReadOnlyList<I2cDataEvent> Items, int Total)> GetPagedByTagIdAsync(
        Guid tagId,
        int page,
        int pageSize,
        CancellationToken ct = default);

    IQueryable<I2cDataEvent> Query();
}