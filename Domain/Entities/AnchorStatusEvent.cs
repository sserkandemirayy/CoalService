using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities;

public class AnchorStatusEvent : BaseEntity
{
    protected AnchorStatusEvent() { }

    public Guid RawEventId { get; private set; }
    public RawEvent RawEvent { get; private set; } = default!;

    public Guid AnchorId { get; private set; }
    public Anchor Anchor { get; private set; } = default!;

    public DateTime EventTimestamp { get; private set; }
    public AnchorStatus Status { get; private set; }
    public AnchorStatus PreviousStatus { get; private set; }
    public string? Reason { get; private set; }

    public static AnchorStatusEvent Create(
        Guid rawEventId,
        Guid anchorId,
        DateTime eventTimestamp,
        AnchorStatus status,
        AnchorStatus previousStatus,
        string? reason = null)
    {
        if (rawEventId == Guid.Empty)
            throw new ArgumentException("RawEventId is required.", nameof(rawEventId));
        if (anchorId == Guid.Empty)
            throw new ArgumentException("AnchorId is required.", nameof(anchorId));

        return new AnchorStatusEvent
        {
            RawEventId = rawEventId,
            AnchorId = anchorId,
            EventTimestamp = eventTimestamp,
            Status = status,
            PreviousStatus = previousStatus,
            Reason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim()
        };
    }
}