using Domain.Abstractions;

namespace Domain.Entities;

public class TagI2cConfigSnapshot : BaseEntity
{
    protected TagI2cConfigSnapshot() { }

    public Guid TagId { get; private set; }
    public Tag Tag { get; private set; } = default!;

    public Guid LastRawEventId { get; private set; }
    public DateTime LastReportedAt { get; private set; }

    public bool Enabled { get; private set; }
    public int ClockSpeed { get; private set; }
    public string DevicesJson { get; private set; } = default!;

    public static TagI2cConfigSnapshot Create(
        Guid tagId,
        Guid lastRawEventId,
        DateTime lastReportedAt,
        bool enabled,
        int clockSpeed,
        string devicesJson)
    {
        if (tagId == Guid.Empty)
            throw new ArgumentException("TagId is required.", nameof(tagId));
        if (lastRawEventId == Guid.Empty)
            throw new ArgumentException("LastRawEventId is required.", nameof(lastRawEventId));

        return new TagI2cConfigSnapshot
        {
            TagId = tagId,
            LastRawEventId = lastRawEventId,
            LastReportedAt = lastReportedAt,
            Enabled = enabled,
            ClockSpeed = clockSpeed,
            DevicesJson = devicesJson
        };
    }

    public void UpdateSnapshot(
        Guid lastRawEventId,
        DateTime lastReportedAt,
        bool enabled,
        int clockSpeed,
        string devicesJson)
    {
        LastRawEventId = lastRawEventId;
        LastReportedAt = lastReportedAt;
        Enabled = enabled;
        ClockSpeed = clockSpeed;
        DevicesJson = devicesJson;
    }
}