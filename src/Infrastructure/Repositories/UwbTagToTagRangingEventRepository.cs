using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UwbTagToTagRangingEventRepository : IUwbTagToTagRangingEventRepository
{
    private readonly AppDbContext _db;

    public UwbTagToTagRangingEventRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<UwbTagToTagRangingEvent?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.UwbTagToTagRangingEvents.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task AddAsync(UwbTagToTagRangingEvent uwbTagToTagRangingEvent, CancellationToken ct = default)
        => await _db.UwbTagToTagRangingEvents.AddAsync(uwbTagToTagRangingEvent, ct);

    public async Task<(IReadOnlyList<UwbTagToTagRangingEvent> Items, int Total)> GetPagedByTagIdAsync(
        Guid tagId,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = _db.UwbTagToTagRangingEvents
            .Where(x => x.TagId == tagId || x.PeerTagId == tagId);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(x => x.EventTimestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public IQueryable<UwbTagToTagRangingEvent> Query() => _db.UwbTagToTagRangingEvents.AsQueryable();
}