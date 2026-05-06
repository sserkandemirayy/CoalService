using Domain.Abstractions;

namespace Domain.Entities;

public class AnchorConfigEvent : BaseEntity
{
    protected AnchorConfigEvent() { }

    public Guid RawEventId { get; private set; }
    public RawEvent RawEvent { get; private set; } = default!;

    public Guid AnchorId { get; private set; }
    public Anchor Anchor { get; private set; } = default!;

    public DateTime EventTimestamp { get; private set; }

    public string FirmwareVersion { get; private set; } = default!;
    public string PositionJson { get; private set; } = default!;
    public string NetworkJson { get; private set; } = default!;
    public string UwbJson { get; private set; } = default!;
    public string BleJson { get; private set; } = default!;
    public long HeartbeatInterval { get; private set; }
    public long ReportInterval { get; private set; }

    public static AnchorConfigEvent Create(
        Guid rawEventId,
        Guid anchorId,
        DateTime eventTimestamp,
        string firmwareVersion,
        string positionJson,
        string networkJson,
        string uwbJson,
        string bleJson,
        long heartbeatInterval,
        long reportInterval)
    {
        if (rawEventId == Guid.Empty)
            throw new ArgumentException("RawEventId is required.", nameof(rawEventId));
        if (anchorId == Guid.Empty)
            throw new ArgumentException("AnchorId is required.", nameof(anchorId));
        if (string.IsNullOrWhiteSpace(firmwareVersion))
            throw new ArgumentException("FirmwareVersion is required.", nameof(firmwareVersion));
        if (string.IsNullOrWhiteSpace(positionJson))
            throw new ArgumentException("PositionJson is required.", nameof(positionJson));
        if (string.IsNullOrWhiteSpace(networkJson))
            throw new ArgumentException("NetworkJson is required.", nameof(networkJson));
        if (string.IsNullOrWhiteSpace(uwbJson))
            throw new ArgumentException("UwbJson is required.", nameof(uwbJson));
        if (string.IsNullOrWhiteSpace(bleJson))
            throw new ArgumentException("BleJson is required.", nameof(bleJson));
        if (heartbeatInterval < 0)
            throw new ArgumentOutOfRangeException(nameof(heartbeatInterval));
        if (reportInterval < 0)
            throw new ArgumentOutOfRangeException(nameof(reportInterval));

        return new AnchorConfigEvent
        {
            RawEventId = rawEventId,
            AnchorId = anchorId,
            EventTimestamp = eventTimestamp,
            FirmwareVersion = firmwareVersion.Trim(),
            PositionJson = positionJson,
            NetworkJson = networkJson,
            UwbJson = uwbJson,
            BleJson = bleJson,
            HeartbeatInterval = heartbeatInterval,
            ReportInterval = reportInterval
        };
    }
}