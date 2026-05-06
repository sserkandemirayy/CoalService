using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProximityEventRepository : IProximityEventRepository
{
    private readonly AppDbContext _db;

    public ProximityEventRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ProximityEvent?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.ProximityEvents.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task AddAsync(ProximityEvent proximityEvent, CancellationToken ct = default)
        => await _db.ProximityEvents.AddAsync(proximityEvent, ct);

    public async Task<(IReadOnlyList<ProximityEvent> Items, int Total)> GetPagedByTagIdAsync(
        Guid tagId,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = _db.ProximityEvents
            .Where(x => x.TagId == tagId || x.PeerTagId == tagId);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(x => x.EventTimestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public IQueryable<ProximityEvent> Query() => _db.ProximityEvents.AsQueryable();
}