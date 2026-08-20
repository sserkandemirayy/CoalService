namespace Domain.Entities;

public class MovementEvent
{
    protected MovementEvent()
    {
    }

    public Guid Id { get; private set; } = Guid.NewGuid();

    public Guid RawEventId { get; private set; }

    public Guid TagId { get; private set; }

    // Historical snapshot
    public string TagExternalId { get; private set; } = default!;
    public string TagCode { get; private set; } = default!;
    public string TagType { get; private set; } = default!;

    public Guid? UserId { get; private set; }

    // Historical snapshot
    public string? UserFullName { get; private set; }
    public string? UserCode { get; private set; }

    public Guid? CompanyId { get; private set; }
    public Guid? BranchId { get; private set; }

    public Guid? FloorMapId { get; private set; }
    public Guid? FloorMapZoneId { get; private set; }

    public decimal X { get; private set; }
    public decimal Y { get; private set; }
    public decimal Z { get; private set; }

    public decimal Accuracy { get; private set; }
    public decimal Confidence { get; private set; }

    public DateTime EventTimestamp { get; private set; }

    /// <summary>
    /// FirstPoint / Interval / Distance / FloorMapChanged / ZoneChanged
    /// </summary>
    public string RecordReason { get; private set; } = default!;

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public static MovementEvent Create(
        Guid rawEventId,
        Guid tagId,
        string tagExternalId,
        string tagCode,
        string tagType,
        Guid? userId,
        string? userFullName,
        string? userCode,
        Guid? companyId,
        Guid? branchId,
        Guid? floorMapId,
        Guid? floorMapZoneId,
        decimal x,
        decimal y,
        decimal z,
        decimal accuracy,
        decimal confidence,
        DateTime eventTimestamp,
        string recordReason)
    {
        if (rawEventId == Guid.Empty)
            throw new ArgumentException(
                "RawEventId is required.",
                nameof(rawEventId));

        if (tagId == Guid.Empty)
            throw new ArgumentException(
                "TagId is required.",
                nameof(tagId));

        if (string.IsNullOrWhiteSpace(tagExternalId))
            throw new ArgumentException(
                "TagExternalId is required.",
                nameof(tagExternalId));

        if (string.IsNullOrWhiteSpace(tagCode))
            throw new ArgumentException(
                "TagCode is required.",
                nameof(tagCode));

        if (string.IsNullOrWhiteSpace(tagType))
            throw new ArgumentException(
                "TagType is required.",
                nameof(tagType));

        if (string.IsNullOrWhiteSpace(recordReason))
            throw new ArgumentException(
                "RecordReason is required.",
                nameof(recordReason));

        return new MovementEvent
        {
            Id = Guid.NewGuid(),

            RawEventId = rawEventId,

            TagId = tagId,

            TagExternalId = tagExternalId.Trim(),
            TagCode = tagCode.Trim(),
            TagType = tagType.Trim(),

            UserId = userId,
            UserFullName = string.IsNullOrWhiteSpace(userFullName)
                ? null
                : userFullName.Trim(),

            UserCode = string.IsNullOrWhiteSpace(userCode)
                ? null
                : userCode.Trim(),

            CompanyId = companyId,
            BranchId = branchId,

            FloorMapId = floorMapId,
            FloorMapZoneId = floorMapZoneId,

            X = x,
            Y = y,
            Z = z,

            Accuracy = accuracy,
            Confidence = confidence,

            EventTimestamp = EnsureUtc(eventTimestamp),

            RecordReason = recordReason.Trim(),

            CreatedAt = DateTime.UtcNow
        };
    }

    private static DateTime EnsureUtc(DateTime value)
    {
        if (value.Kind == DateTimeKind.Utc)
            return value;

        return DateTime.SpecifyKind(
            value,
            DateTimeKind.Utc);
    }
}