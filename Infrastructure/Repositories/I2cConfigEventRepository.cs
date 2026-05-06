using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class I2cConfigEventRepository : II2cConfigEventRepository
{
    private readonly AppDbContext _db;
    public I2cConfigEventRepository(AppDbContext db) => _db = db;

    public async Task<I2cConfigEvent?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.I2cConfigEvents.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task AddAsync(I2cConfigEvent entity, CancellationToken ct = default)
        => await _db.I2cConfigEvents.AddAsync(entity, ct);

    public async Task<(IReadOnlyList<I2cConfigEvent> Items, int Total)> GetPagedByTagIdAsync(
        Guid tagId, int page, int pageSize, CancellationToken ct = default)
    {
        var query = _db.I2cConfigEvents.Where(x => x.TagId == tagId);
        var total = await query.CountAsync(ct);
        var items = await query.OrderByDescending(x => x.EventTimestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public IQueryable<I2cConfigEvent> Query() => _db.I2cConfigEvents.AsQueryable();
}