using Domain.Entities;

namespace Domain.Abstractions;

public interface ILocationEventRepository
{
    Task<LocationEvent?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(LocationEvent locationEvent, CancellationToken ct = default);

    Task<(IReadOnlyList<LocationEvent> Items, int Total)> GetPagedByTagIdAsync(
        Guid tagId,
        int page,
        int pageSize,
        CancellationToken ct = default);

    Task<(IReadOnlyList<LocationEvent> Items, int Total)> GetPagedHistoryAsync(
        Guid tagId,
        DateTime? from,
        DateTime? to,
        int page,
        int pageSize,
        CancellationToken ct = default);

    Task<LocationEvent?> GetLatestByTagIdAsync(Guid tagId, CancellationToken ct = default);

    IQueryable<LocationEvent> Query();
}