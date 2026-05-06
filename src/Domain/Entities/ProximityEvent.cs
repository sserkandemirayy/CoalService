using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities;

public class ProximityEvent : BaseEntity
{
    protected ProximityEvent() { }

    public Guid RawEventId { get; private set; }
    public RawEvent RawEvent { get; private set; } = default!;

    public Guid TagId { get; private set; }
    public Tag Tag { get; private set; } = default!;

    public Guid PeerTagId { get; private set; }
    public Tag PeerTag { get; private set; } = default!;

    public DateTime EventTimestamp { get; private set; }

    public decimal Distance { get; private set; }
    public decimal Threshold { get; private set; }
    public ProximitySeverity Severity { get; private set; }

    public static ProximityEvent Create(
        Guid rawEventId,
        Guid tagId,
        Guid peerTagId,
        DateTime eventTimestamp,
        decimal distance,
        decimal threshold,
        ProximitySeverity severity)
    {
        if (rawEventId == Guid.Empty)
            throw new ArgumentException("RawEventId is required.", nameof(rawEventId));
        if (tagId == Guid.Empty)
            throw new ArgumentException("TagId is required.", nameof(tagId));
        if (peerTagId == Guid.Empty)
            throw new ArgumentException("PeerTagId is required.", nameof(peerTagId));
        if (distance < 0)
            throw new ArgumentOutOfRangeException(nameof(distance));
        if (threshold < 0)
            throw new ArgumentOutOfRangeException(nameof(threshold));

        return new ProximityEvent
        {
            RawEventId = rawEventId,
            TagId = tagId,
            PeerTagId = peerTagId,
            EventTimestamp = eventTimestamp,
            Distance = distance,
            Threshold = threshold,
            Severity = severity
        };
    }
}