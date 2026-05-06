using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AnchorHeartbeatEventRepository : IAnchorHeartbeatEventRepository
{
    private readonly AppDbContext _db;

    public AnchorHeartbeatEventRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<AnchorHeartbeatEvent?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.AnchorHeartbeatEvents.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task AddAsync(AnchorHeartbeatEvent heartbeatEvent, CancellationToken ct = default)
        => await _db.AnchorHeartbeatEvents.AddAsync(heartbeatEvent, ct);

    public async Task<(IReadOnlyList<AnchorHeartbeatEvent> Items, int Total)> GetPagedByAnchorIdAsync(
        Guid anchorId,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = _db.AnchorHeartbeatEvents.Where(x => x.AnchorId == anchorId);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(x => x.EventTimestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public IQueryable<AnchorHeartbeatEvent> Query() => _db.AnchorHeartbeatEvents.AsQueryable();
}