using Domain.Entities;

namespace Domain.Abstractions;

public interface IUwbTagToTagRangingEventRepository
{
    Task<UwbTagToTagRangingEvent?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(UwbTagToTagRangingEvent uwbTagToTagRangingEvent, CancellationToken ct = default);

    Task<(IReadOnlyList<UwbTagToTagRangingEvent> Items, int Total)> GetPagedByTagIdAsync(
        Guid tagId,
        int page,
        int pageSize,
        CancellationToken ct = default);

    IQueryable<UwbTagToTagRangingEvent> Query();
}