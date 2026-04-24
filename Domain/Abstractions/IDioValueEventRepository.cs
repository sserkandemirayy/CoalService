using Domain.Entities;

namespace Domain.Abstractions;

public interface IDioValueEventRepository
{
    Task<DioValueEvent?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(DioValueEvent entity, CancellationToken ct = default);

    Task<(IReadOnlyList<DioValueEvent> Items, int Total)> GetPagedByTagIdAsync(
        Guid tagId,
        int page,
        int pageSize,
        CancellationToken ct = default);

    IQueryable<DioValueEvent> Query();
}