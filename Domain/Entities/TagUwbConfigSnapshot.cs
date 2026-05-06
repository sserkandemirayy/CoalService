using Domain.Abstractions;

namespace Domain.Entities;

public class TagUwbConfigSnapshot : BaseEntity
{
    protected TagUwbConfigSnapshot() { }

    public Guid TagId { get; private set; }
    public Tag Tag { get; private set; } = default!;

    public Guid LastRawEventId { get; private set; }
    public DateTime LastReportedAt { get; private set; }

    public bool Enabled { get; private set; }
    public int Channel { get; private set; }
    public decimal TxPower { get; private set; }
    public long RangingInterval { get; private set; }
    public bool TagToTagEnabled { get; private set; }
    public long TagToTagInterval { get; private set; }

    public static TagUwbConfigSnapshot Create(
        Guid tagId,
        Guid lastRawEventId,
        DateTime lastReportedAt,
        bool enabled,
        int channel,
        decimal txPower,
        long rangingInterval,
        bool tagToTagEnabled,
        long tagToTagInterval)
    {
        if (tagId == Guid.Empty)
            throw new ArgumentException("TagId is required.", nameof(tagId));
        if (lastRawEventId == Guid.Empty)
            throw new ArgumentException("LastRawEventId is required.", nameof(lastRawEventId));

        return new TagUwbConfigSnapshot
        {
            TagId = tagId,
            LastRawEventId = lastRawEventId,
            LastReportedAt = lastReportedAt,
            Enabled = enabled,
            Channel = channel,
            TxPower = txPower,
            RangingInterval = rangingInterval,
            TagToTagEnabled = tagToTagEnabled,
            TagToTagInterval = tagToTagInterval
        };
    }

    public void UpdateSnapshot(
        Guid lastRawEventId,
        DateTime lastReportedAt,
        bool enabled,
        int channel,
        decimal txPower,
        long rangingInterval,
        bool tagToTagEnabled,
        long tagToTagInterval)
    {
        LastRawEventId = lastRawEventId;
        LastReportedAt = lastReportedAt;
        Enabled = enabled;
        Channel = channel;
        TxPower = txPower;
        RangingInterval = rangingInterval;
        TagToTagEnabled = tagToTagEnabled;
        TagToTagInterval = tagToTagInterval;
    }
}