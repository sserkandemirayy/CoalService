using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities;

public class TagDioConfigSnapshot : BaseEntity
{
    protected TagDioConfigSnapshot() { }

    public Guid TagId { get; private set; }
    public Tag Tag { get; private set; } = default!;

    public int Pin { get; private set; }

    public Guid LastRawEventId { get; private set; }
    public DateTime LastReportedAt { get; private set; }

    public DioDirection Direction { get; private set; }
    public bool PeriodicReportEnabled { get; private set; }
    public long PeriodicReportInterval { get; private set; }
    public bool ReportOnChange { get; private set; }
    public string LowPassFilterJson { get; private set; } = default!;

    public static TagDioConfigSnapshot Create(
        Guid tagId,
        int pin,
        Guid lastRawEventId,
        DateTime lastReportedAt,
        DioDirection direction,
        bool periodicReportEnabled,
        long periodicReportInterval,
        bool reportOnChange,
        string lowPassFilterJson)
    {
        if (tagId == Guid.Empty)
            throw new ArgumentException("TagId is required.", nameof(tagId));
        if (pin < 0)
            throw new ArgumentOutOfRangeException(nameof(pin));
        if (lastRawEventId == Guid.Empty)
            throw new ArgumentException("LastRawEventId is required.", nameof(lastRawEventId));

        return new TagDioConfigSnapshot
        {
            TagId = tagId,
            Pin = pin,
            LastRawEventId = lastRawEventId,
            LastReportedAt = lastReportedAt,
            Direction = direction,
            PeriodicReportEnabled = periodicReportEnabled,
            PeriodicReportInterval = periodicReportInterval,
            ReportOnChange = reportOnChange,
            LowPassFilterJson = lowPassFilterJson
        };
    }

    public void UpdateSnapshot(
        Guid lastRawEventId,
        DateTime lastReportedAt,
        DioDirection direction,
        bool periodicReportEnabled,
        long periodicReportInterval,
        bool reportOnChange,
        string lowPassFilterJson)
    {
        LastRawEventId = lastRawEventId;
        LastReportedAt = lastReportedAt;
        Direction = direction;
        PeriodicReportEnabled = periodicReportEnabled;
        PeriodicReportInterval = periodicReportInterval;
        ReportOnChange = reportOnChange;
        LowPassFilterJson = lowPassFilterJson;
    }
}