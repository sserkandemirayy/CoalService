using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class BleAdvertisementEventRepository : IBleAdvertisementEventRepository
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public BleAdvertisementEventRepository(AppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<BleAdvertisementEvent?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await Scoped().FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task AddAsync(BleAdvertisementEvent entity, CancellationToken ct = default)
        => await _db.BleAdvertisementEvents.AddAsync(entity, ct);

    public async Task<(IReadOnlyList<BleAdvertisementEvent> Items, int Total)> GetPagedByTagIdAsync(
        Guid tagId,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = Scoped().Where(x => x.TagId == tagId);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(x => x.EventTimestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<(IReadOnlyList<BleAdvertisementEvent> Items, int Total)> GetPagedByAnchorIdAsync(
        Guid anchorId,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = Scoped().Where(x => x.AnchorId == anchorId);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(x => x.EventTimestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    private IQueryable<BleAdvertisementEvent> Scoped()
    {
        var tags = RepositoryScope.Tags(_db, _currentUser);
        var anchors = RepositoryScope.Anchors(_db, _currentUser);
        return _db.BleAdvertisementEvents.Where(x => tags.Any(t => t.Id == x.TagId) || anchors.Any(a => a.Id == x.AnchorId));
    }

    public IQueryable<BleAdvertisementEvent> Query() => _db.BleAdvertisementEvents.AsQueryable();
}