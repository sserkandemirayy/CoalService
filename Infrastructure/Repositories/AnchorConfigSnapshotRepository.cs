using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AnchorConfigSnapshotRepository : IAnchorConfigSnapshotRepository
{
    private readonly AppDbContext _db;
    public AnchorConfigSnapshotRepository(AppDbContext db) => _db = db;

    public async Task<AnchorConfigSnapshot?> GetByAnchorIdAsync(Guid anchorId, CancellationToken ct = default)
        => await _db.AnchorConfigSnapshots.FirstOrDefaultAsync(x => x.AnchorId == anchorId, ct);

    public async Task AddAsync(AnchorConfigSnapshot entity, CancellationToken ct = default)
        => await _db.AnchorConfigSnapshots.AddAsync(entity, ct);

    public Task UpdateAsync(AnchorConfigSnapshot entity, CancellationToken ct = default)
    {
        _db.AnchorConfigSnapshots.Update(entity);
        return Task.CompletedTask;
    }

    public IQueryable<AnchorConfigSnapshot> Query() => _db.AnchorConfigSnapshots.AsQueryable();
}