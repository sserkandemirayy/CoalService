using Domain.Abstractions;

namespace Domain.Entities;

public class TagDioValueSnapshot : BaseEntity
{
    protected TagDioValueSnapshot() { }

    public Guid TagId { get; private set; }
    public Tag Tag { get; private set; } = default!;

    public int Pin { get; private set; }

    public Guid LastRawEventId { get; private set; }
    public DateTime LastReportedAt { get; private set; }

    public bool Value { get; private set; }

    public static TagDioValueSnapshot Create(
        Guid tagId,
        int pin,
        Guid lastRawEventId,
        DateTime lastReportedAt,
        bool value)
    {
        if (tagId == Guid.Empty)
            throw new ArgumentException("TagId is required.", nameof(tagId));
        if (pin < 0)
            throw new ArgumentOutOfRangeException(nameof(pin));
        if (lastRawEventId == Guid.Empty)
            throw new ArgumentException("LastRawEventId is required.", nameof(lastRawEventId));

        return new TagDioValueSnapshot
        {
            TagId = tagId,
            Pin = pin,
            LastRawEventId = lastRawEventId,
            LastReportedAt = lastReportedAt,
            Value = value
        };
    }

    public void UpdateSnapshot(
        Guid lastRawEventId,
        DateTime lastReportedAt,
        bool value)
    {
        LastRawEventId = lastRawEventId;
        LastReportedAt = lastReportedAt;
        Value = value;
    }
}