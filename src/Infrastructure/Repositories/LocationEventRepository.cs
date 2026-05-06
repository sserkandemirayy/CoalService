using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class LocationEventRepository : ILocationEventRepository
{
    private readonly AppDbContext _db;

    public LocationEventRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<LocationEvent?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.LocationEvents.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task AddAsync(LocationEvent locationEvent, CancellationToken ct = default)
        => await _db.LocationEvents.AddAsync(locationEvent, ct);

    public async Task<(IReadOnlyList<LocationEvent> Items, int Total)> GetPagedByTagIdAsync(
        Guid tagId,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = _db.LocationEvents.Where(x => x.TagId == tagId);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(x => x.EventTimestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<(IReadOnlyList<LocationEvent> Items, int Total)> GetPagedHistoryAsync(
        Guid tagId,
        DateTime? from,
        DateTime? to,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = _db.LocationEvents.Where(x => x.TagId == tagId);

        if (from.HasValue)
            query = query.Where(x => x.EventTimestamp >= from.Value);

        if (to.HasValue)
            query = query.Where(x => x.EventTimestamp <= to.Value);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(x => x.EventTimestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<LocationEvent?> GetLatestByTagIdAsync(Guid tagId, CancellationToken ct = default)
        => await _db.LocationEvents
            .Where(x => x.TagId == tagId)
            .OrderByDescending(x => x.EventTimestamp)
            .FirstOrDefaultAsync(ct);

    public IQueryable<LocationEvent> Query() => _db.LocationEvents.AsQueryable();
}