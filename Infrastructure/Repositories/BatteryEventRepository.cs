using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class BatteryEventRepository : IBatteryEventRepository
{
    private readonly AppDbContext _db;

    public BatteryEventRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<BatteryEvent?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.BatteryEvents.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task AddAsync(BatteryEvent batteryEvent, CancellationToken ct = default)
        => await _db.BatteryEvents.AddAsync(batteryEvent, ct);

    public async Task<(IReadOnlyList<BatteryEvent> Items, int Total)> GetPagedByTagIdAsync(
        Guid tagId,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = _db.BatteryEvents.Where(x => x.TagId == tagId);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(x => x.EventTimestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public IQueryable<BatteryEvent> Query() => _db.BatteryEvents.AsQueryable();
}