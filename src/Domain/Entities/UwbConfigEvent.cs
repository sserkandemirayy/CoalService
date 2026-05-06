using Domain.Abstractions;

namespace Domain.Entities;

public class UwbConfigEvent : BaseEntity
{
    protected UwbConfigEvent() { }

    public Guid RawEventId { get; private set; }
    public RawEvent RawEvent { get; private set; } = default!;

    public Guid TagId { get; private set; }
    public Tag Tag { get; private set; } = default!;

    public DateTime EventTimestamp { get; private set; }

    public bool Enabled { get; private set; }
    public int Channel { get; private set; }
    public decimal TxPower { get; private set; }
    public long RangingInterval { get; private set; }
    public bool TagToTagEnabled { get; private set; }
    public long TagToTagInterval { get; private set; }

    public static UwbConfigEvent Create(
        Guid rawEventId,
        Guid tagId,
        DateTime eventTimestamp,
        bool enabled,
        int channel,
        decimal txPower,
        long rangingInterval,
        bool tagToTagEnabled,
        long tagToTagInterval)
    {
        if (rawEventId == Guid.Empty)
            throw new ArgumentException("RawEventId is required.", nameof(rawEventId));
        if (tagId == Guid.Empty)
            throw new ArgumentException("TagId is required.", nameof(tagId));
        if (channel < 0)
            throw new ArgumentOutOfRangeException(nameof(channel));
        if (rangingInterval < 0)
            throw new ArgumentOutOfRangeException(nameof(rangingInterval));
        if (tagToTagInterval < 0)
            throw new ArgumentOutOfRangeException(nameof(tagToTagInterval));

        return new UwbConfigEvent
        {
            RawEventId = rawEventId,
            TagId = tagId,
            EventTimestamp = eventTimestamp,
            Enabled = enabled,
            Channel = channel,
            TxPower = txPower,
            RangingInterval = rangingInterval,
            TagToTagEnabled = tagToTagEnabled,
            TagToTagInterval = tagToTagInterval
        };
    }
}