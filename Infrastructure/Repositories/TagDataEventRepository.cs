using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TagDataEventRepository : ITagDataEventRepository
{
    private readonly AppDbContext _db;

    public TagDataEventRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<TagDataEvent?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.TagDataEvents.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task AddAsync(TagDataEvent tagDataEvent, CancellationToken ct = default)
        => await _db.TagDataEvents.AddAsync(tagDataEvent, ct);

    public async Task<(IReadOnlyList<TagDataEvent> Items, int Total)> GetPagedByTagIdAsync(
        Guid tagId,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = _db.TagDataEvents.Where(x => x.TagId == tagId);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(x => x.EventTimestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public IQueryable<TagDataEvent> Query() => _db.TagDataEvents.AsQueryable();
}