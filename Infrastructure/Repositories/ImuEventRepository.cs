using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ImuEventRepository : IImuEventRepository
{
    private readonly AppDbContext _db;

    public ImuEventRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ImuEvent?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.ImuEvents.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task AddAsync(ImuEvent imuEvent, CancellationToken ct = default)
        => await _db.ImuEvents.AddAsync(imuEvent, ct);

    public async Task<(IReadOnlyList<ImuEvent> Items, int Total)> GetPagedByTagIdAsync(
        Guid tagId,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = _db.ImuEvents.Where(x => x.TagId == tagId);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(x => x.EventTimestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public IQueryable<ImuEvent> Query() => _db.ImuEvents.AsQueryable();
}