using Domain.Abstractions;

namespace Domain.Entities;

public class AnchorHeartbeatEvent : BaseEntity
{
    protected AnchorHeartbeatEvent() { }

    public Guid RawEventId { get; private set; }
    public RawEvent RawEvent { get; private set; } = default!;

    public Guid AnchorId { get; private set; }
    public Anchor Anchor { get; private set; } = default!;

    public DateTime EventTimestamp { get; private set; }
    public string IpAddress { get; private set; } = default!;

    public static AnchorHeartbeatEvent Create(
        Guid rawEventId,
        Guid anchorId,
        DateTime eventTimestamp,
        string ipAddress)
    {
        if (rawEventId == Guid.Empty)
            throw new ArgumentException("RawEventId is required.", nameof(rawEventId));
        if (anchorId == Guid.Empty)
            throw new ArgumentException("AnchorId is required.", nameof(anchorId));
        if (string.IsNullOrWhiteSpace(ipAddress))
            throw new ArgumentException("IpAddress is required.", nameof(ipAddress));

        return new AnchorHeartbeatEvent
        {
            RawEventId = rawEventId,
            AnchorId = anchorId,
            EventTimestamp = eventTimestamp,
            IpAddress = ipAddress.Trim()
        };
    }
}