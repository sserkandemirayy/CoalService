using Domain.Abstractions;

namespace Domain.Entities;

public class UwbTagToTagRangingEvent : BaseEntity
{
    protected UwbTagToTagRangingEvent() { }

    public Guid RawEventId { get; private set; }
    public RawEvent RawEvent { get; private set; } = default!;

    public Guid TagId { get; private set; }
    public Tag Tag { get; private set; } = default!;

    public Guid PeerTagId { get; private set; }
    public Tag PeerTag { get; private set; } = default!;

    public DateTime EventTimestamp { get; private set; }
    public decimal Distance { get; private set; }

    public static UwbTagToTagRangingEvent Create(
        Guid rawEventId,
        Guid tagId,
        Guid peerTagId,
        DateTime eventTimestamp,
        decimal distance)
    {
        if (rawEventId == Guid.Empty)
            throw new ArgumentException("RawEventId is required.", nameof(rawEventId));
        if (tagId == Guid.Empty)
            throw new ArgumentException("TagId is required.", nameof(tagId));
        if (peerTagId == Guid.Empty)
            throw new ArgumentException("PeerTagId is required.", nameof(peerTagId));
        if (distance < 0)
            throw new ArgumentOutOfRangeException(nameof(distance));

        return new UwbTagToTagRangingEvent
        {
            RawEventId = rawEventId,
            TagId = tagId,
            PeerTagId = peerTagId,
            EventTimestamp = eventTimestamp,
            Distance = distance
        };
    }
}