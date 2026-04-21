using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class EmergencyEventRepository : IEmergencyEventRepository
{
    private readonly AppDbContext _db;

    public EmergencyEventRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<EmergencyEvent?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.EmergencyEvents.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task AddAsync(EmergencyEvent emergencyEvent, CancellationToken ct = default)
        => await _db.EmergencyEvents.AddAsync(emergencyEvent, ct);

    public async Task<(IReadOnlyList<EmergencyEvent> Items, int Total)> GetPagedByTagIdAsync(
        Guid tagId,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = _db.EmergencyEvents.Where(x => x.TagId == tagId);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(x => x.EventTimestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public IQueryable<EmergencyEvent> Query() => _db.EmergencyEvents.AsQueryable();
}