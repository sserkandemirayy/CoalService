using Domain.Entities;

namespace Domain.Abstractions;

public interface ITagUwbConfigSnapshotRepository
{
    Task<TagUwbConfigSnapshot?> GetByTagIdAsync(Guid tagId, CancellationToken ct = default);
    Task AddAsync(TagUwbConfigSnapshot entity, CancellationToken ct = default);
    Task UpdateAsync(TagUwbConfigSnapshot entity, CancellationToken ct = default);
    IQueryable<TagUwbConfigSnapshot> Query();
}