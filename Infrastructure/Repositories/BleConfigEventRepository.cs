using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class BleConfigEventRepository : IBleConfigEventRepository
{
    private readonly AppDbContext _db;
    public BleConfigEventRepository(AppDbContext db) => _db = db;

    public async Task<BleConfigEvent?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.BleConfigEvents.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task AddAsync(BleConfigEvent entity, CancellationToken ct = default)
        => await _db.BleConfigEvents.AddAsync(entity, ct);

    public async Task<(IReadOnlyList<BleConfigEvent> Items, int Total)> GetPagedByTagIdAsync(
        Guid tagId, int page, int pageSize, CancellationToken ct = default)
    {
        var query = _db.BleConfigEvents.Where(x => x.TagId == tagId);
        var total = await query.CountAsync(ct);
        var items = await query.OrderByDescending(x => x.EventTimestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public IQueryable<BleConfigEvent> Query() => _db.BleConfigEvents.AsQueryable();
}