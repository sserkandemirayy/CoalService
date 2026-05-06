using Domain.Entities;

namespace Domain.Abstractions;

public interface ITagDioConfigSnapshotRepository
{
    Task<TagDioConfigSnapshot?> GetByTagIdAndPinAsync(Guid tagId, int pin, CancellationToken ct = default);
    Task<IReadOnlyList<TagDioConfigSnapshot>> GetByTagIdAsync(Guid tagId, CancellationToken ct = default);
    Task AddAsync(TagDioConfigSnapshot entity, CancellationToken ct = default);
    Task UpdateAsync(TagDioConfigSnapshot entity, CancellationToken ct = default);
    IQueryable<TagDioConfigSnapshot> Query();
}