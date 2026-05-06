using Domain.Abstractions;

namespace Domain.Entities;

public class CurrentLocation : BaseEntity
{
    protected CurrentLocation() { }

    public Guid TagId { get; private set; }
    public Tag Tag { get; private set; } = default!;

    public Guid? UserId { get; private set; }
    public User? User { get; private set; }

    public decimal X { get; private set; }
    public decimal Y { get; private set; }
    public decimal Z { get; private set; }

    public decimal Accuracy { get; private set; }
    public decimal Confidence { get; private set; }

    public DateTime LastEventAt { get; private set; }
    public Guid LastRawEventId { get; private set; }

    public int LastKnownAnchorCount { get; private set; }

    public static CurrentLocation Create(
        Guid tagId,
        Guid? userId,
        decimal x,
        decimal y,
        decimal z,
        decimal accuracy,
        decimal confidence,
        DateTime lastEventAt,
        Guid lastRawEventId,
        int lastKnownAnchorCount = 0)
    {
        if (tagId == Guid.Empty)
            throw new ArgumentException("TagId is required.", nameof(tagId));
        if (lastRawEventId == Guid.Empty)
            throw new ArgumentException("LastRawEventId is required.", nameof(lastRawEventId));

        return new CurrentLocation
        {
            TagId = tagId,
            UserId = userId,
            X = x,
            Y = y,
            Z = z,
            Accuracy = accuracy,
            Confidence = confidence,
            LastEventAt = lastEventAt,
            LastRawEventId = lastRawEventId,
            LastKnownAnchorCount = lastKnownAnchorCount
        };
    }

    public void UpdateFromLocation(
        Guid? userId,
        decimal x,
        decimal y,
        decimal z,
        decimal accuracy,
        decimal confidence,
        DateTime lastEventAt,
        Guid lastRawEventId,
        int lastKnownAnchorCount)
    {
        UserId = userId;
        X = x;
        Y = y;
        Z = z;
        Accuracy = accuracy;
        Confidence = confidence;
        LastEventAt = lastEventAt;
        LastRawEventId = lastRawEventId;
        LastKnownAnchorCount = lastKnownAnchorCount;
    }
}