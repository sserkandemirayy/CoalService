using Domain.Entities;

namespace Domain.Abstractions;

public interface ITagDioValueSnapshotRepository
{
    Task<TagDioValueSnapshot?> GetByTagIdAndPinAsync(Guid tagId, int pin, CancellationToken ct = default);
    Task<IReadOnlyList<TagDioValueSnapshot>> GetByTagIdAsync(Guid tagId, CancellationToken ct = default);
    Task AddAsync(TagDioValueSnapshot entity, CancellationToken ct = default);
    Task UpdateAsync(TagDioValueSnapshot entity, CancellationToken ct = default);
    IQueryable<TagDioValueSnapshot> Query();
}