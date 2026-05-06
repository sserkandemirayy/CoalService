using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TagDioConfigSnapshotRepository : ITagDioConfigSnapshotRepository
{
    private readonly AppDbContext _db;
    public TagDioConfigSnapshotRepository(AppDbContext db) => _db = db;

    public async Task<TagDioConfigSnapshot?> GetByTagIdAndPinAsync(Guid tagId, int pin, CancellationToken ct = default)
        => await _db.TagDioConfigSnapshots.FirstOrDefaultAsync(x => x.TagId == tagId && x.Pin == pin, ct);

    public async Task<IReadOnlyList<TagDioConfigSnapshot>> GetByTagIdAsync(Guid tagId, CancellationToken ct = default)
        => await _db.TagDioConfigSnapshots.Where(x => x.TagId == tagId).OrderBy(x => x.Pin).ToListAsync(ct);

    public async Task AddAsync(TagDioConfigSnapshot entity, CancellationToken ct = default)
        => await _db.TagDioConfigSnapshots.AddAsync(entity, ct);

    public Task UpdateAsync(TagDioConfigSnapshot entity, CancellationToken ct = default)
    {
        _db.TagDioConfigSnapshots.Update(entity);
        return Task.CompletedTask;
    }

    public IQueryable<TagDioConfigSnapshot> Query() => _db.TagDioConfigSnapshots.AsQueryable();
}