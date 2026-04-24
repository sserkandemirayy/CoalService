using Domain.Abstractions;

namespace Domain.Entities;

public class DioValueEvent : BaseEntity
{
    protected DioValueEvent() { }

    public Guid RawEventId { get; private set; }
    public RawEvent RawEvent { get; private set; } = default!;

    public Guid TagId { get; private set; }
    public Tag Tag { get; private set; } = default!;

    public DateTime EventTimestamp { get; private set; }
    public int Pin { get; private set; }
    public bool Value { get; private set; }

    public static DioValueEvent Create(
        Guid rawEventId,
        Guid tagId,
        DateTime eventTimestamp,
        int pin,
        bool value)
    {
        if (rawEventId == Guid.Empty)
            throw new ArgumentException("RawEventId is required.", nameof(rawEventId));
        if (tagId == Guid.Empty)
            throw new ArgumentException("TagId is required.", nameof(tagId));
        if (pin < 0)
            throw new ArgumentOutOfRangeException(nameof(pin));

        return new DioValueEvent
        {
            RawEventId = rawEventId,
            TagId = tagId,
            EventTimestamp = eventTimestamp,
            Pin = pin,
            Value = value
        };
    }
}