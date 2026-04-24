using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class I2cDataEventRepository : II2cDataEventRepository
{
    private readonly AppDbContext _db;

    public I2cDataEventRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<I2cDataEvent?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.I2cDataEvents.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task AddAsync(I2cDataEvent entity, CancellationToken ct = default)
        => await _db.I2cDataEvents.AddAsync(entity, ct);

    public async Task<(IReadOnlyList<I2cDataEvent> Items, int Total)> GetPagedByTagIdAsync(
        Guid tagId,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = _db.I2cDataEvents.Where(x => x.TagId == tagId);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(x => x.EventTimestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public IQueryable<I2cDataEvent> Query() => _db.I2cDataEvents.AsQueryable();
}