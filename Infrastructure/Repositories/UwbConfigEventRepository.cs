using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UwbConfigEventRepository : IUwbConfigEventRepository
{
    private readonly AppDbContext _db;
    public UwbConfigEventRepository(AppDbContext db) => _db = db;

    public async Task<UwbConfigEvent?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.UwbConfigEvents.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task AddAsync(UwbConfigEvent entity, CancellationToken ct = default)
        => await _db.UwbConfigEvents.AddAsync(entity, ct);

    public async Task<(IReadOnlyList<UwbConfigEvent> Items, int Total)> GetPagedByTagIdAsync(
        Guid tagId, int page, int pageSize, CancellationToken ct = default)
    {
        var query = _db.UwbConfigEvents.Where(x => x.TagId == tagId);
        var total = await query.CountAsync(ct);
        var items = await query.OrderByDescending(x => x.EventTimestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public IQueryable<UwbConfigEvent> Query() => _db.UwbConfigEvents.AsQueryable();
}