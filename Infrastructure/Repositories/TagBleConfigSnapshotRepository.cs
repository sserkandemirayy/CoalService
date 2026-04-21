using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TagBleConfigSnapshotRepository : ITagBleConfigSnapshotRepository
{
    private readonly AppDbContext _db;
    public TagBleConfigSnapshotRepository(AppDbContext db) => _db = db;

    public async Task<TagBleConfigSnapshot?> GetByTagIdAsync(Guid tagId, CancellationToken ct = default)
        => await _db.TagBleConfigSnapshots.FirstOrDefaultAsync(x => x.TagId == tagId, ct);

    public async Task AddAsync(TagBleConfigSnapshot entity, CancellationToken ct = default)
        => await _db.TagBleConfigSnapshots.AddAsync(entity, ct);

    public Task UpdateAsync(TagBleConfigSnapshot entity, CancellationToken ct = default)
    {
        _db.TagBleConfigSnapshots.Update(entity);
        return Task.CompletedTask;
    }

    public IQueryable<TagBleConfigSnapshot> Query() => _db.TagBleConfigSnapshots.AsQueryable();
}