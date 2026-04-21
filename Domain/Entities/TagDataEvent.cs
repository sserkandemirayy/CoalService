using Domain.Abstractions;

namespace Domain.Entities;

public class TagDataEvent : BaseEntity
{
    protected TagDataEvent() { }

    public Guid RawEventId { get; private set; }
    public RawEvent RawEvent { get; private set; } = default!;

    public Guid AnchorId { get; private set; }
    public Anchor Anchor { get; private set; } = default!;

    public Guid TagId { get; private set; }
    public Tag Tag { get; private set; } = default!;

    public DateTime EventTimestamp { get; private set; }
    public string? ReportedTagType { get; private set; }

    public static TagDataEvent Create(
        Guid rawEventId,
        Guid anchorId,
        Guid tagId,
        DateTime eventTimestamp,
        string? reportedTagType)
    {
        if (rawEventId == Guid.Empty)
            throw new ArgumentException("RawEventId is required.", nameof(rawEventId));
        if (anchorId == Guid.Empty)
            throw new ArgumentException("AnchorId is required.", nameof(anchorId));
        if (tagId == Guid.Empty)
            throw new ArgumentException("TagId is required.", nameof(tagId));

        return new TagDataEvent
        {
            RawEventId = rawEventId,
            AnchorId = anchorId,
            TagId = tagId,
            EventTimestamp = eventTimestamp,
            ReportedTagType = string.IsNullOrWhiteSpace(reportedTagType) ? null : reportedTagType.Trim()
        };
    }
}