using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AnchorConfigEventRepository : IAnchorConfigEventRepository
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserService _currentUser;
    public AnchorConfigEventRepository(AppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<AnchorConfigEvent?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await Scoped().FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task AddAsync(AnchorConfigEvent entity, CancellationToken ct = default)
        => await _db.AnchorConfigEvents.AddAsync(entity, ct);

    public async Task<(IReadOnlyList<AnchorConfigEvent> Items, int Total)> GetPagedByAnchorIdAsync(
        Guid anchorId, int page, int pageSize, CancellationToken ct = default)
    {
        var query = Scoped().Where(x => x.AnchorId == anchorId);
        var total = await query.CountAsync(ct);
        var items = await query.OrderByDescending(x => x.EventTimestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    private IQueryable<AnchorConfigEvent> Scoped()
    {
        var anchors = RepositoryScope.Anchors(_db, _currentUser);
        return _db.AnchorConfigEvents.Where(x => anchors.Any(a => a.Id == x.AnchorId));
    }

    public IQueryable<AnchorConfigEvent> Query() => _db.AnchorConfigEvents.AsQueryable();
}