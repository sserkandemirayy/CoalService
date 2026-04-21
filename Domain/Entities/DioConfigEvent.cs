using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities;

public class DioConfigEvent : BaseEntity
{
    protected DioConfigEvent() { }

    public Guid RawEventId { get; private set; }
    public RawEvent RawEvent { get; private set; } = default!;

    public Guid TagId { get; private set; }
    public Tag Tag { get; private set; } = default!;

    public DateTime EventTimestamp { get; private set; }

    public int Pin { get; private set; }
    public DioDirection Direction { get; private set; }
    public bool PeriodicReportEnabled { get; private set; }
    public long PeriodicReportInterval { get; private set; }
    public bool ReportOnChange { get; private set; }
    public string LowPassFilterJson { get; private set; } = default!;

    public static DioConfigEvent Create(
        Guid rawEventId,
        Guid tagId,
        DateTime eventTimestamp,
        int pin,
        DioDirection direction,
        bool periodicReportEnabled,
        long periodicReportInterval,
        bool reportOnChange,
        string lowPassFilterJson)
    {
        if (rawEventId == Guid.Empty)
            throw new ArgumentException("RawEventId is required.", nameof(rawEventId));
        if (tagId == Guid.Empty)
            throw new ArgumentException("TagId is required.", nameof(tagId));
        if (pin < 0)
            throw new ArgumentOutOfRangeException(nameof(pin));
        if (periodicReportInterval < 0)
            throw new ArgumentOutOfRangeException(nameof(periodicReportInterval));
        if (string.IsNullOrWhiteSpace(lowPassFilterJson))
            throw new ArgumentException("LowPassFilterJson is required.", nameof(lowPassFilterJson));

        return new DioConfigEvent
        {
            RawEventId = rawEventId,
            TagId = tagId,
            EventTimestamp = eventTimestamp,
            Pin = pin,
            Direction = direction,
            PeriodicReportEnabled = periodicReportEnabled,
            PeriodicReportInterval = periodicReportInterval,
            ReportOnChange = reportOnChange,
            LowPassFilterJson = lowPassFilterJson
        };
    }
}