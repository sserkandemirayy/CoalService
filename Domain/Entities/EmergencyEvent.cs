using Domain.Abstractions;

namespace Domain.Entities;

public class EmergencyEvent : BaseEntity
{
    protected EmergencyEvent() { }

    public Guid RawEventId { get; private set; }
    public RawEvent RawEvent { get; private set; } = default!;

    public Guid TagId { get; private set; }
    public Tag Tag { get; private set; } = default!;

    public DateTime EventTimestamp { get; private set; }

    public static EmergencyEvent Create(Guid rawEventId, Guid tagId, DateTime eventTimestamp)
    {
        if (rawEventId == Guid.Empty)
            throw new ArgumentException("RawEventId is required.", nameof(rawEventId));
        if (tagId == Guid.Empty)
            throw new ArgumentException("TagId is required.", nameof(tagId));

        return new EmergencyEvent
        {
            RawEventId = rawEventId,
            TagId = tagId,
            EventTimestamp = eventTimestamp
        };
    }
}