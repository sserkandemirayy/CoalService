using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities;

public class ImuEvent : BaseEntity
{
    protected ImuEvent() { }

    public Guid RawEventId { get; private set; }
    public RawEvent RawEvent { get; private set; } = default!;

    public Guid TagId { get; private set; }
    public Tag Tag { get; private set; } = default!;

    public DateTime EventTimestamp { get; private set; }
    public ImuEventType EventType { get; private set; }

    public static ImuEvent Create(
        Guid rawEventId,
        Guid tagId,
        DateTime eventTimestamp,
        ImuEventType eventType)
    {
        if (rawEventId == Guid.Empty)
            throw new ArgumentException("RawEventId is required.", nameof(rawEventId));
        if (tagId == Guid.Empty)
            throw new ArgumentException("TagId is required.", nameof(tagId));

        return new ImuEvent
        {
            RawEventId = rawEventId,
            TagId = tagId,
            EventTimestamp = eventTimestamp,
            EventType = eventType
        };
    }
}