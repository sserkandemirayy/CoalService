using Domain.Abstractions;

namespace Domain.Entities;

public class UwbRangingEvent : BaseEntity
{
    protected UwbRangingEvent() { }

    public Guid RawEventId { get; private set; }
    public RawEvent RawEvent { get; private set; } = default!;

    public Guid AnchorId { get; private set; }
    public Anchor Anchor { get; private set; } = default!;

    public Guid TagId { get; private set; }
    public Tag Tag { get; private set; } = default!;

    public DateTime EventTimestamp { get; private set; }
    public decimal Distance { get; private set; }

    public static UwbRangingEvent Create(
        Guid rawEventId,
        Guid anchorId,
        Guid tagId,
        DateTime eventTimestamp,
        decimal distance)
    {
        if (rawEventId == Guid.Empty)
            throw new ArgumentException("RawEventId is required.", nameof(rawEventId));
        if (anchorId == Guid.Empty)
            throw new ArgumentException("AnchorId is required.", nameof(anchorId));
        if (tagId == Guid.Empty)
            throw new ArgumentException("TagId is required.", nameof(tagId));
        if (distance < 0)
            throw new ArgumentOutOfRangeException(nameof(distance));

        return new UwbRangingEvent
        {
            RawEventId = rawEventId,
            AnchorId = anchorId,
            TagId = tagId,
            EventTimestamp = eventTimestamp,
            Distance = distance
        };
    }
}