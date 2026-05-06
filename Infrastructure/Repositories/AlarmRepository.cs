using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AlarmRepository : IAlarmRepository
{
    private readonly AppDbContext _db;

    public AlarmRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Alarm?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.Alarms.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<IEnumerable<Alarm>> GetActiveAlarmsAsync(CancellationToken ct = default)
        => await _db.Alarms
            .Where(x => x.Status == AlarmStatus.Active || x.Status == AlarmStatus.Acknowledged)
            .OrderByDescending(x => x.StartedAt)
            .ToListAsync(ct);

    public async Task<IEnumerable<Alarm>> GetByTagIdAsync(Guid tagId, CancellationToken ct = default)
        => await _db.Alarms
            .Where(x => x.TagId == tagId || x.PeerTagId == tagId)
            .OrderByDescending(x => x.StartedAt)
            .ToListAsync(ct);

    public async Task<IEnumerable<Alarm>> GetByAnchorIdAsync(Guid anchorId, CancellationToken ct = default)
        => await _db.Alarms
            .Where(x => x.AnchorId == anchorId)
            .OrderByDescending(x => x.StartedAt)
            .ToListAsync(ct);

    public async Task<bool> HasActiveAlarmAsync(
        AlarmType alarmType,
        Guid? tagId = null,
        Guid? peerTagId = null,
        Guid? anchorId = null,
        CancellationToken ct = default)
    {
        var query = _db.Alarms.Where(x =>
            x.AlarmType == alarmType &&
            (x.Status == AlarmStatus.Active || x.Status == AlarmStatus.Acknowledged));

        if (tagId.HasValue)
            query = query.Where(x => x.TagId == tagId.Value);

        if (peerTagId.HasValue)
            query = query.Where(x => x.PeerTagId == peerTagId.Value);

        if (anchorId.HasValue)
            query = query.Where(x => x.AnchorId == anchorId.Value);

        return await query.AnyAsync(ct);
    }

    public async Task AddAsync(Alarm alarm, CancellationToken ct = default)
        => await _db.Alarms.AddAsync(alarm, ct);

    public Task UpdateAsync(Alarm alarm, CancellationToken ct = default)
    {
        _db.Alarms.Update(alarm);
        return Task.CompletedTask;
    }

    public IQueryable<Alarm> Query() => _db.Alarms.AsQueryable();
}