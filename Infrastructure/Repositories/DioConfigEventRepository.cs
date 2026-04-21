using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class DioConfigEventRepository : IDioConfigEventRepository
{
    private readonly AppDbContext _db;
    public DioConfigEventRepository(AppDbContext db) => _db = db;

    public async Task<DioConfigEvent?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.DioConfigEvents.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task AddAsync(DioConfigEvent entity, CancellationToken ct = default)
        => await _db.DioConfigEvents.AddAsync(entity, ct);

    public async Task<(IReadOnlyList<DioConfigEvent> Items, int Total)> GetPagedByTagIdAsync(
        Guid tagId, int page, int pageSize, CancellationToken ct = default)
    {
        var query = _db.DioConfigEvents.Where(x => x.TagId == tagId);
        var total = await query.CountAsync(ct);
        var items = await query.OrderByDescending(x => x.EventTimestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public IQueryable<DioConfigEvent> Query() => _db.DioConfigEvents.AsQueryable();
}