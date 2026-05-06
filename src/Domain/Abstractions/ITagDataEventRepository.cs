using Domain.Entities;

namespace Domain.Abstractions;

public interface ITagDataEventRepository
{
    Task<TagDataEvent?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(TagDataEvent tagDataEvent, CancellationToken ct = default);

    Task<(IReadOnlyList<TagDataEvent> Items, int Total)> GetPagedByTagIdAsync(
        Guid tagId,
        int page,
        int pageSize,
        CancellationToken ct = default);

    IQueryable<TagDataEvent> Query();
}