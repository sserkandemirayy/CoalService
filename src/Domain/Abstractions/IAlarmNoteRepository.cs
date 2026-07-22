using Domain.Entities;

namespace Domain.Abstractions;

public interface IAlarmNoteRepository
{
    Task AddAsync(AlarmNote note, CancellationToken ct = default);

    Task<IReadOnlyList<AlarmNote>> GetByAlarmIdAsync(
        Guid alarmId,
        CancellationToken ct = default);
}