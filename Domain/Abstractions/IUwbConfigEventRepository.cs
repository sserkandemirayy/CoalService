using Domain.Entities;

namespace Domain.Abstractions;

public interface IUwbConfigEventRepository
{
    Task<UwbConfigEvent?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(UwbConfigEvent entity, CancellationToken ct = default);

    Task<(IReadOnlyList<UwbConfigEvent> Items, int Total)> GetPagedByTagIdAsync(
        Guid tagId,
        int page,
        int pageSize,
        CancellationToken ct = default);

    IQueryable<UwbConfigEvent> Query();
}