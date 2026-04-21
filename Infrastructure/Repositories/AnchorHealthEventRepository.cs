using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AnchorHealthEventRepository : IAnchorHealthEventRepository
{
    private readonly AppDbContext _db;

    public AnchorHealthEventRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<AnchorHealthEvent?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.AnchorHealthEvents.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task AddAsync(AnchorHealthEvent healthEvent, CancellationToken ct = default)
        => await _db.AnchorHealthEvents.AddAsync(healthEvent, ct);

    public async Task<(IReadOnlyList<AnchorHealthEvent> Items, int Total)> GetPagedByAnchorIdAsync(
        Guid anchorId,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = _db.AnchorHealthEvents.Where(x => x.AnchorId == anchorId);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(x => x.EventTimestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public IQueryable<AnchorHealthEvent> Query() => _db.AnchorHealthEvents.AsQueryable();
}