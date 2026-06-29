using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AnchorConfigSnapshotRepository : IAnchorConfigSnapshotRepository
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserService _currentUser;
    public AnchorConfigSnapshotRepository(AppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<AnchorConfigSnapshot?> GetByAnchorIdAsync(Guid anchorId, CancellationToken ct = default)
        => await Scoped().FirstOrDefaultAsync(x => x.AnchorId == anchorId, ct);

    public async Task AddAsync(AnchorConfigSnapshot entity, CancellationToken ct = default)
        => await _db.AnchorConfigSnapshots.AddAsync(entity, ct);

    public Task UpdateAsync(AnchorConfigSnapshot entity, CancellationToken ct = default)
    {
        _db.AnchorConfigSnapshots.Update(entity);
        return Task.CompletedTask;
    }

    private IQueryable<AnchorConfigSnapshot> Scoped()
    {
        var anchors = RepositoryScope.Anchors(_db, _currentUser);
        return _db.AnchorConfigSnapshots.Where(x => anchors.Any(a => a.Id == x.AnchorId));
    }

    public IQueryable<AnchorConfigSnapshot> Query() => _db.AnchorConfigSnapshots.AsQueryable();
}