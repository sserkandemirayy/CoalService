using Domain.Entities;

namespace Domain.Abstractions;

public interface IDioConfigEventRepository
{
    Task<DioConfigEvent?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(DioConfigEvent entity, CancellationToken ct = default);

    Task<(IReadOnlyList<DioConfigEvent> Items, int Total)> GetPagedByTagIdAsync(
        Guid tagId,
        int page,
        int pageSize,
        CancellationToken ct = default);

    IQueryable<DioConfigEvent> Query();
}