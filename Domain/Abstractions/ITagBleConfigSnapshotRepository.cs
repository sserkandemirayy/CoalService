using Domain.Entities;

namespace Domain.Abstractions;

public interface ITagBleConfigSnapshotRepository
{
    Task<TagBleConfigSnapshot?> GetByTagIdAsync(Guid tagId, CancellationToken ct = default);
    Task AddAsync(TagBleConfigSnapshot entity, CancellationToken ct = default);
    Task UpdateAsync(TagBleConfigSnapshot entity, CancellationToken ct = default);
    IQueryable<TagBleConfigSnapshot> Query();
}