using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TagI2cConfigSnapshotRepository : ITagI2cConfigSnapshotRepository
{
    private readonly AppDbContext _db;
    public TagI2cConfigSnapshotRepository(AppDbContext db) => _db = db;

    public async Task<TagI2cConfigSnapshot?> GetByTagIdAsync(Guid tagId, CancellationToken ct = default)
        => await _db.TagI2cConfigSnapshots.FirstOrDefaultAsync(x => x.TagId == tagId, ct);

    public async Task AddAsync(TagI2cConfigSnapshot entity, CancellationToken ct = default)
        => await _db.TagI2cConfigSnapshots.AddAsync(entity, ct);

    public Task UpdateAsync(TagI2cConfigSnapshot entity, CancellationToken ct = default)
    {
        _db.TagI2cConfigSnapshots.Update(entity);
        return Task.CompletedTask;
    }

    public IQueryable<TagI2cConfigSnapshot> Query() => _db.TagI2cConfigSnapshots.AsQueryable();
}