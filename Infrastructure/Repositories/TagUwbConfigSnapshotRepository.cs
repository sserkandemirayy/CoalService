using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TagUwbConfigSnapshotRepository : ITagUwbConfigSnapshotRepository
{
    private readonly AppDbContext _db;
    public TagUwbConfigSnapshotRepository(AppDbContext db) => _db = db;

    public async Task<TagUwbConfigSnapshot?> GetByTagIdAsync(Guid tagId, CancellationToken ct = default)
        => await _db.TagUwbConfigSnapshots.FirstOrDefaultAsync(x => x.TagId == tagId, ct);

    public async Task AddAsync(TagUwbConfigSnapshot entity, CancellationToken ct = default)
        => await _db.TagUwbConfigSnapshots.AddAsync(entity, ct);

    public Task UpdateAsync(TagUwbConfigSnapshot entity, CancellationToken ct = default)
    {
        _db.TagUwbConfigSnapshots.Update(entity);
        return Task.CompletedTask;
    }

    public IQueryable<TagUwbConfigSnapshot> Query() => _db.TagUwbConfigSnapshots.AsQueryable();
}