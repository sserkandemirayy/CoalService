using Domain.Abstractions;

namespace Domain.Entities;

public class TagBleConfigSnapshot : BaseEntity
{
    protected TagBleConfigSnapshot() { }

    public Guid TagId { get; private set; }
    public Tag Tag { get; private set; } = default!;

    public Guid LastRawEventId { get; private set; }
    public DateTime LastReportedAt { get; private set; }

    public bool Enabled { get; private set; }
    public decimal TxPower { get; private set; }
    public long AdvertisementInterval { get; private set; }
    public bool MeshEnabled { get; private set; }

    public static TagBleConfigSnapshot Create(
        Guid tagId,
        Guid lastRawEventId,
        DateTime lastReportedAt,
        bool enabled,
        decimal txPower,
        long advertisementInterval,
        bool meshEnabled)
    {
        if (tagId == Guid.Empty)
            throw new ArgumentException("TagId is required.", nameof(tagId));
        if (lastRawEventId == Guid.Empty)
            throw new ArgumentException("LastRawEventId is required.", nameof(lastRawEventId));

        return new TagBleConfigSnapshot
        {
            TagId = tagId,
            LastRawEventId = lastRawEventId,
            LastReportedAt = lastReportedAt,
            Enabled = enabled,
            TxPower = txPower,
            AdvertisementInterval = advertisementInterval,
            MeshEnabled = meshEnabled
        };
    }

    public void UpdateSnapshot(
        Guid lastRawEventId,
        DateTime lastReportedAt,
        bool enabled,
        decimal txPower,
        long advertisementInterval,
        bool meshEnabled)
    {
        LastRawEventId = lastRawEventId;
        LastReportedAt = lastReportedAt;
        Enabled = enabled;
        TxPower = txPower;
        AdvertisementInterval = advertisementInterval;
        MeshEnabled = meshEnabled;
    }
}