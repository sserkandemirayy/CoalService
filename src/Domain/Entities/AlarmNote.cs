using Domain.Abstractions;

namespace Domain.Entities;

public class AlarmNote : BaseEntity
{
    protected AlarmNote() { }

    public Guid AlarmId { get; private set; }
    public Alarm Alarm { get; private set; } = default!;

    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!;

    public string Note { get; private set; } = default!;

    public static AlarmNote Create(
        Guid alarmId,
        Guid userId,
        string note)
    {
        if (string.IsNullOrWhiteSpace(note))
            throw new ArgumentException("Note is required.", nameof(note));

        return new AlarmNote
        {
            AlarmId = alarmId,
            UserId = userId,
            Note = note.Trim()
        };
    }

    public void Update(string note)
    {
        if (string.IsNullOrWhiteSpace(note))
            throw new ArgumentException(nameof(note));

        Note = note.Trim();
    }
}