using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TagDioValueSnapshotRepository : ITagDioValueSnapshotRepository
{
    private readonly AppDbContext _db;

    public TagDioValueSnapshotRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<TagDioValueSnapshot?> GetByTagIdAndPinAsync(Guid tagId, int pin, CancellationToken ct = default)
        => await _db.TagDioValueSnapshots.FirstOrDefaultAsync(x => x.TagId == tagId && x.Pin == pin, ct);

    public async Task<IReadOnlyList<TagDioValueSnapshot>> GetByTagIdAsync(Guid tagId, CancellationToken ct = default)
        => await _db.TagDioValueSnapshots
            .Where(x => x.TagId == tagId)
            .OrderBy(x => x.Pin)
            .ToListAsync(ct);

    public async Task AddAsync(TagDioValueSnapshot entity, CancellationToken ct = default)
        => await _db.TagDioValueSnapshots.AddAsync(entity, ct);

    public Task UpdateAsync(TagDioValueSnapshot entity, CancellationToken ct = default)
    {
        _db.TagDioValueSnapshots.Update(entity);
        return Task.CompletedTask;
    }

    public IQueryable<TagDioValueSnapshot> Query() => _db.TagDioValueSnapshots.AsQueryable();
}