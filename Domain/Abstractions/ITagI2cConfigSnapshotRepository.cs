using Domain.Entities;

namespace Domain.Abstractions;

public interface ITagI2cConfigSnapshotRepository
{
    Task<TagI2cConfigSnapshot?> GetByTagIdAsync(Guid tagId, CancellationToken ct = default);
    Task AddAsync(TagI2cConfigSnapshot entity, CancellationToken ct = default);
    Task UpdateAsync(TagI2cConfigSnapshot entity, CancellationToken ct = default);
    IQueryable<TagI2cConfigSnapshot> Query();
}