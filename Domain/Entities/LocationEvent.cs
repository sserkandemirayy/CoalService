using Domain.Abstractions;

namespace Domain.Entities;

public class LocationEvent : BaseEntity
{
    protected LocationEvent() { }

    public Guid RawEventId { get; private set; }
    public RawEvent RawEvent { get; private set; } = default!;

    public Guid TagId { get; private set; }
    public Tag Tag { get; private set; } = default!;

    public DateTime EventTimestamp { get; private set; }

    public decimal X { get; private set; }
    public decimal Y { get; private set; }
    public decimal Z { get; private set; }

    public decimal Accuracy { get; private set; }
    public decimal Confidence { get; private set; }

    public string UsedAnchorsJson { get; private set; } = "[]";

    public static LocationEvent Create(
        Guid rawEventId,
        Guid tagId,
        DateTime eventTimestamp,
        decimal x,
        decimal y,
        decimal z,
        decimal accuracy,
        decimal confidence,
        string usedAnchorsJson)
    {
        if (rawEventId == Guid.Empty)
            throw new ArgumentException("RawEventId is required.", nameof(rawEventId));
        if (tagId == Guid.Empty)
            throw new ArgumentException("TagId is required.", nameof(tagId));
        if (accuracy < 0)
            throw new ArgumentOutOfRangeException(nameof(accuracy));
        if (confidence < 0 || confidence > 100)
            throw new ArgumentOutOfRangeException(nameof(confidence));

        return new LocationEvent
        {
            RawEventId = rawEventId,
            TagId = tagId,
            EventTimestamp = eventTimestamp,
            X = x,
            Y = y,
            Z = z,
            Accuracy = accuracy,
            Confidence = confidence,
            UsedAnchorsJson = string.IsNullOrWhiteSpace(usedAnchorsJson) ? "[]" : usedAnchorsJson
        };
    }
}