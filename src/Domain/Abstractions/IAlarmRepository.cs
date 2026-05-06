using Domain.Entities;
using Domain.Enums;

namespace Domain.Abstractions;

public interface IAlarmRepository
{
    Task<Alarm?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Alarm>> GetActiveAlarmsAsync(CancellationToken ct = default);
    Task<IEnumerable<Alarm>> GetByTagIdAsync(Guid tagId, CancellationToken ct = default);
    Task<IEnumerable<Alarm>> GetByAnchorIdAsync(Guid anchorId, CancellationToken ct = default);
    Task<bool> HasActiveAlarmAsync(
        AlarmType alarmType,
        Guid? tagId = null,
        Guid? peerTagId = null,
        Guid? anchorId = null,
        CancellationToken ct = default);

    Task AddAsync(Alarm alarm, CancellationToken ct = default);
    Task UpdateAsync(Alarm alarm, CancellationToken ct = default);
    IQueryable<Alarm> Query();
}