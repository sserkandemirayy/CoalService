using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class DioValueEventRepository : IDioValueEventRepository
{
    private readonly AppDbContext _db;

    public DioValueEventRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<DioValueEvent?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.DioValueEvents.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task AddAsync(DioValueEvent entity, CancellationToken ct = default)
        => await _db.DioValueEvents.AddAsync(entity, ct);

    public async Task<(IReadOnlyList<DioValueEvent> Items, int Total)> GetPagedByTagIdAsync(
        Guid tagId,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = _db.DioValueEvents.Where(x => x.TagId == tagId);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(x => x.EventTimestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public IQueryable<DioValueEvent> Query() => _db.DioValueEvents.AsQueryable();
}