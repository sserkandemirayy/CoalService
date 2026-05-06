using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities;

public class Alarm : BaseEntity
{
    protected Alarm() { }

    public Guid? RawEventId { get; private set; }
    public RawEvent? RawEvent { get; private set; }

    public AlarmType AlarmType { get; private set; }
    public AlarmSeverity Severity { get; private set; }
    public AlarmStatus Status { get; private set; }

    public Guid? TagId { get; private set; }
    public Tag? Tag { get; private set; }

    public Guid? PeerTagId { get; private set; }
    public Tag? PeerTag { get; private set; }

    public Guid? AnchorId { get; private set; }
    public Anchor? Anchor { get; private set; }

    public Guid? UserId { get; private set; }
    public User? User { get; private set; }

    public DateTime StartedAt { get; private set; }
    public DateTime? EndedAt { get; private set; }

    public DateTime? AcknowledgedAt { get; private set; }
    public Guid? AcknowledgedBy { get; private set; }

    public string Title { get; private set; } = default!;
    public string? Description { get; private set; }
    public string? DataJson { get; private set; }

    public static Alarm Create(
        AlarmType alarmType,
        AlarmSeverity severity,
        string title,
        DateTime startedAt,
        Guid? rawEventId = null,
        Guid? tagId = null,
        Guid? peerTagId = null,
        Guid? anchorId = null,
        Guid? userId = null,
        string? description = null,
        string? dataJson = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));

        return new Alarm
        {
            RawEventId = rawEventId,
            AlarmType = alarmType,
            Severity = severity,
            Status = AlarmStatus.Active,
            TagId = tagId,
            PeerTagId = peerTagId,
            AnchorId = anchorId,
            UserId = userId,
            StartedAt = startedAt,
            Title = title.Trim(),
            Description = description?.Trim(),
            DataJson = dataJson
        };
    }

    public void Acknowledge(Guid acknowledgedByUserId, DateTime? at = null)
    {
        if (Status == AlarmStatus.Resolved || Status == AlarmStatus.Closed)
            return;

        Status = AlarmStatus.Acknowledged;
        AcknowledgedBy = acknowledgedByUserId;
        AcknowledgedAt = at ?? DateTime.UtcNow;
    }

    public void Resolve(DateTime? endedAt = null)
    {
        Status = AlarmStatus.Resolved;
        EndedAt = endedAt ?? DateTime.UtcNow;
    }

    public void Close(DateTime? endedAt = null)
    {
        Status = AlarmStatus.Closed;
        EndedAt = endedAt ?? DateTime.UtcNow;
    }
}