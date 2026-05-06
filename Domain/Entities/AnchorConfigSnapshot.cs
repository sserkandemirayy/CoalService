using Domain.Abstractions;

namespace Domain.Entities;

public class AnchorConfigSnapshot : BaseEntity
{
    protected AnchorConfigSnapshot() { }

    public Guid AnchorId { get; private set; }
    public Anchor Anchor { get; private set; } = default!;

    public Guid LastRawEventId { get; private set; }
    public DateTime LastReportedAt { get; private set; }

    public string FirmwareVersion { get; private set; } = default!;
    public string PositionJson { get; private set; } = default!;
    public string NetworkJson { get; private set; } = default!;
    public string UwbJson { get; private set; } = default!;
    public string BleJson { get; private set; } = default!;
    public long HeartbeatInterval { get; private set; }
    public long ReportInterval { get; private set; }

    public static AnchorConfigSnapshot Create(
        Guid anchorId,
        Guid lastRawEventId,
        DateTime lastReportedAt,
        string firmwareVersion,
        string positionJson,
        string networkJson,
        string uwbJson,
        string bleJson,
        long heartbeatInterval,
        long reportInterval)
    {
        if (anchorId == Guid.Empty)
            throw new ArgumentException("AnchorId is required.", nameof(anchorId));
        if (lastRawEventId == Guid.Empty)
            throw new ArgumentException("LastRawEventId is required.", nameof(lastRawEventId));

        return new AnchorConfigSnapshot
        {
            AnchorId = anchorId,
            LastRawEventId = lastRawEventId,
            LastReportedAt = lastReportedAt,
            FirmwareVersion = firmwareVersion.Trim(),
            PositionJson = positionJson,
            NetworkJson = networkJson,
            UwbJson = uwbJson,
            BleJson = bleJson,
            HeartbeatInterval = heartbeatInterval,
            ReportInterval = reportInterval
        };
    }

    public void UpdateSnapshot(
        Guid lastRawEventId,
        DateTime lastReportedAt,
        string firmwareVersion,
        string positionJson,
        string networkJson,
        string uwbJson,
        string bleJson,
        long heartbeatInterval,
        long reportInterval)
    {
        LastRawEventId = lastRawEventId;
        LastReportedAt = lastReportedAt;
        FirmwareVersion = firmwareVersion.Trim();
        PositionJson = positionJson;
        NetworkJson = networkJson;
        UwbJson = uwbJson;
        BleJson = bleJson;
        HeartbeatInterval = heartbeatInterval;
        ReportInterval = reportInterval;
    }
}