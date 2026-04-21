using Domain.Abstractions;

namespace Domain.Entities;

public class I2cConfigEvent : BaseEntity
{
    protected I2cConfigEvent() { }

    public Guid RawEventId { get; private set; }
    public RawEvent RawEvent { get; private set; } = default!;

    public Guid TagId { get; private set; }
    public Tag Tag { get; private set; } = default!;

    public DateTime EventTimestamp { get; private set; }

    public bool Enabled { get; private set; }
    public int ClockSpeed { get; private set; }
    public string DevicesJson { get; private set; } = default!;

    public static I2cConfigEvent Create(
        Guid rawEventId,
        Guid tagId,
        DateTime eventTimestamp,
        bool enabled,
        int clockSpeed,
        string devicesJson)
    {
        if (rawEventId == Guid.Empty)
            throw new ArgumentException("RawEventId is required.", nameof(rawEventId));
        if (tagId == Guid.Empty)
            throw new ArgumentException("TagId is required.", nameof(tagId));
        if (clockSpeed < 0)
            throw new ArgumentOutOfRangeException(nameof(clockSpeed));
        if (string.IsNullOrWhiteSpace(devicesJson))
            throw new ArgumentException("DevicesJson is required.", nameof(devicesJson));

        return new I2cConfigEvent
        {
            RawEventId = rawEventId,
            TagId = tagId,
            EventTimestamp = eventTimestamp,
            Enabled = enabled,
            ClockSpeed = clockSpeed,
            DevicesJson = devicesJson
        };
    }
}