using Domain.Abstractions;

namespace Domain.Entities;

public class BleConfigEvent : BaseEntity
{
    protected BleConfigEvent() { }

    public Guid RawEventId { get; private set; }
    public RawEvent RawEvent { get; private set; } = default!;

    public Guid TagId { get; private set; }
    public Tag Tag { get; private set; } = default!;

    public DateTime EventTimestamp { get; private set; }

    public bool Enabled { get; private set; }
    public decimal TxPower { get; private set; }
    public long AdvertisementInterval { get; private set; }
    public bool MeshEnabled { get; private set; }

    public static BleConfigEvent Create(
        Guid rawEventId,
        Guid tagId,
        DateTime eventTimestamp,
        bool enabled,
        decimal txPower,
        long advertisementInterval,
        bool meshEnabled)
    {
        if (rawEventId == Guid.Empty)
            throw new ArgumentException("RawEventId is required.", nameof(rawEventId));
        if (tagId == Guid.Empty)
            throw new ArgumentException("TagId is required.", nameof(tagId));
        if (advertisementInterval < 0)
            throw new ArgumentOutOfRangeException(nameof(advertisementInterval));

        return new BleConfigEvent
        {
            RawEventId = rawEventId,
            TagId = tagId,
            EventTimestamp = eventTimestamp,
            Enabled = enabled,
            TxPower = txPower,
            AdvertisementInterval = advertisementInterval,
            MeshEnabled = meshEnabled
        };
    }
}