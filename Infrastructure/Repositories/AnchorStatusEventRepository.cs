using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AnchorStatusEventRepository : IAnchorStatusEventRepository
{
    private readonly AppDbContext _db;

    public AnchorStatusEventRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<AnchorStatusEvent?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.AnchorStatusEvents.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task AddAsync(AnchorStatusEvent statusEvent, CancellationToken ct = default)
        => await _db.AnchorStatusEvents.AddAsync(statusEvent, ct);

    public async Task<(IReadOnlyList<AnchorStatusEvent> Items, int Total)> GetPagedByAnchorIdAsync(
        Guid anchorId,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = _db.AnchorStatusEvents.Where(x => x.AnchorId == anchorId);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(x => x.EventTimestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public IQueryable<AnchorStatusEvent> Query() => _db.AnchorStatusEvents.AsQueryable();
}