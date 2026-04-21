namespace Application.DTOs.EventProcessing;

public sealed record UsedAnchorPayloadDto(string AnchorId);

public sealed record LocationCalculatedPayloadDto(
    Guid Id,
    string Type,
    long Timestamp,
    string TagId,
    decimal X,
    decimal Y,
    decimal Z,
    decimal Accuracy,
    decimal Confidence,
    List<UsedAnchorPayloadDto> UsedAnchors
);

public sealed record EmergencyButtonPressedPayloadDto(
    Guid Id,
    string Type,
    long Timestamp,
    string TagId
);

public sealed record ProximityAlertRaisedPayloadDto(
    Guid Id,
    string Type,
    long Timestamp,
    string TagId,
    string PeerTagId,
    decimal Distance,
    decimal Threshold,
    string Severity
);

public sealed record ImuEventDetectedPayloadDto(
    Guid Id,
    string Type,
    long Timestamp,
    string TagId,
    string EventType
);

public sealed record BatteryLevelReportedPayloadDto(
    Guid Id,
    string Type,
    long Timestamp,
    string AnchorId,
    string TagId,
    int BatteryLevel
);

public sealed record AnchorHeartbeatReceivedPayloadDto(
    Guid Id,
    string Type,
    long Timestamp,
    string AnchorId,
    string IpAddress
);

public sealed record AnchorStatusChangedPayloadDto(
    Guid Id,
    string Type,
    long Timestamp,
    string AnchorId,
    string Status,
    string PreviousStatus,
    string? Reason
);

public sealed record RawEventDto(
    Guid Id,
    Guid ExternalEventId,
    string EventType,
    DateTime EventTimestamp,
    string? TagExternalId,
    string? AnchorExternalId,
    string PayloadJson,
    DateTime ReceivedAt,
    string ProcessingStatus,
    DateTime? ProcessedAt,
    string? ErrorMessage
);

public sealed record LocationEventDto(
    Guid Id,
    Guid RawEventId,
    Guid TagId,
    DateTime EventTimestamp,
    decimal X,
    decimal Y,
    decimal Z,
    decimal Accuracy,
    decimal Confidence,
    string UsedAnchorsJson
);

public sealed record BatteryEventDto(
    Guid Id,
    Guid RawEventId,
    Guid TagId,
    Guid AnchorId,
    DateTime EventTimestamp,
    int BatteryLevel
);

public sealed record ImuEventDto(
    Guid Id,
    Guid RawEventId,
    Guid TagId,
    DateTime EventTimestamp,
    string EventType
);

public sealed record ProximityEventDto(
    Guid Id,
    Guid RawEventId,
    Guid TagId,
    Guid PeerTagId,
    DateTime EventTimestamp,
    decimal Distance,
    decimal Threshold,
    string Severity
);

public sealed record EmergencyEventDto(
    Guid Id,
    Guid RawEventId,
    Guid TagId,
    DateTime EventTimestamp
);

public sealed record AnchorHeartbeatEventDto(
    Guid Id,
    Guid RawEventId,
    Guid AnchorId,
    DateTime EventTimestamp,
    string IpAddress
);

public sealed record AnchorStatusEventDto(
    Guid Id,
    Guid RawEventId,
    Guid AnchorId,
    DateTime EventTimestamp,
    string Status,
    string PreviousStatus,
    string? Reason
);

public sealed record AnchorHealthReportedPayloadDto(
    Guid Id,
    string Type,
    long Timestamp,
    string AnchorId,
    long Uptime,
    decimal Temperature,
    decimal CpuUsage,
    decimal MemoryUsage,
    int TagCount,
    decimal PacketLossRate
);

public sealed record TagDataReceivedPayloadDto(
    Guid Id,
    string Type,
    long Timestamp,
    string AnchorId,
    string TagId,
    string? TagType
);

public sealed record UwbRangingCompletedPayloadDto(
    Guid Id,
    string Type,
    long Timestamp,
    string AnchorId,
    string TagId,
    decimal Distance
);

public sealed record UwbTagToTagRangingCompletedPayloadDto(
    Guid Id,
    string Type,
    long Timestamp,
    string TagId,
    string PeerTagId,
    decimal Distance
);

public sealed record AnchorHealthEventDto(
    Guid Id,
    Guid RawEventId,
    Guid AnchorId,
    DateTime EventTimestamp,
    long Uptime,
    decimal Temperature,
    decimal CpuUsage,
    decimal MemoryUsage,
    int TagCount,
    decimal PacketLossRate
);

public sealed record TagDataEventDto(
    Guid Id,
    Guid RawEventId,
    Guid AnchorId,
    Guid TagId,
    DateTime EventTimestamp,
    string? ReportedTagType
);

public sealed record UwbRangingEventDto(
    Guid Id,
    Guid RawEventId,
    Guid AnchorId,
    Guid TagId,
    DateTime EventTimestamp,
    decimal Distance
);

public sealed record UwbTagToTagRangingEventDto(
    Guid Id,
    Guid RawEventId,
    Guid TagId,
    Guid PeerTagId,
    DateTime EventTimestamp,
    decimal Distance
);

public sealed record AnchorPositionPayloadDto(decimal X, decimal Y, decimal Z);

public sealed record AnchorNetworkPayloadDto(
    string MacAddress,
    string IpAddress,
    string Netmask,
    string Gateway,
    string Dns,
    bool DhcpEnabled
);

public sealed record AnchorUwbPayloadDto(
    bool Enabled,
    int Channel,
    decimal TxPower
);

public sealed record AnchorBlePayloadDto(
    bool Enabled,
    decimal TxPower
);

public sealed record AnchorConfigReportedPayloadDto(
    Guid Id,
    string Type,
    long Timestamp,
    string AnchorId,
    string FirmwareVersion,
    AnchorPositionPayloadDto Position,
    AnchorNetworkPayloadDto Network,
    AnchorUwbPayloadDto Uwb,
    AnchorBlePayloadDto Ble,
    long HeartbeatInterval,
    long ReportInterval
);

public sealed record BleConfigReportedPayloadDto(
    Guid Id,
    string Type,
    long Timestamp,
    string TagId,
    bool Enabled,
    decimal TxPower,
    long AdvertisementInterval,
    bool MeshEnabled
);

public sealed record UwbConfigReportedPayloadDto(
    Guid Id,
    string Type,
    long Timestamp,
    string TagId,
    bool Enabled,
    int Channel,
    decimal TxPower,
    long RangingInterval,
    bool TagToTagEnabled,
    long TagToTagInterval
);

public sealed record DioLowPassFilterPayloadDto(
    bool Enabled,
    long TimeConstant
);

public sealed record DioConfigReportedPayloadDto(
    Guid Id,
    string Type,
    long Timestamp,
    string TagId,
    int Pin,
    string Direction,
    bool PeriodicReportEnabled,
    long PeriodicReportInterval,
    bool ReportOnChange,
    DioLowPassFilterPayloadDto LowPassFilter
);

public sealed record I2cDevicePayloadDto(
    string? Address,
    string? Name,
    string? Type,
    string? MetadataJson
);

public sealed record I2cConfigReportedPayloadDto(
    Guid Id,
    string Type,
    long Timestamp,
    string TagId,
    bool Enabled,
    int ClockSpeed,
    List<I2cDevicePayloadDto> Devices
);

public sealed record AnchorConfigEventDto(
    Guid Id,
    Guid RawEventId,
    Guid AnchorId,
    DateTime EventTimestamp,
    string FirmwareVersion,
    string PositionJson,
    string NetworkJson,
    string UwbJson,
    string BleJson,
    long HeartbeatInterval,
    long ReportInterval
);

public sealed record AnchorConfigSnapshotDto(
    Guid Id,
    Guid AnchorId,
    Guid LastRawEventId,
    DateTime LastReportedAt,
    string FirmwareVersion,
    string PositionJson,
    string NetworkJson,
    string UwbJson,
    string BleJson,
    long HeartbeatInterval,
    long ReportInterval
);

public sealed record BleConfigEventDto(
    Guid Id,
    Guid RawEventId,
    Guid TagId,
    DateTime EventTimestamp,
    bool Enabled,
    decimal TxPower,
    long AdvertisementInterval,
    bool MeshEnabled
);

public sealed record TagBleConfigSnapshotDto(
    Guid Id,
    Guid TagId,
    Guid LastRawEventId,
    DateTime LastReportedAt,
    bool Enabled,
    decimal TxPower,
    long AdvertisementInterval,
    bool MeshEnabled
);

public sealed record UwbConfigEventDto(
    Guid Id,
    Guid RawEventId,
    Guid TagId,
    DateTime EventTimestamp,
    bool Enabled,
    int Channel,
    decimal TxPower,
    long RangingInterval,
    bool TagToTagEnabled,
    long TagToTagInterval
);

public sealed record TagUwbConfigSnapshotDto(
    Guid Id,
    Guid TagId,
    Guid LastRawEventId,
    DateTime LastReportedAt,
    bool Enabled,
    int Channel,
    decimal TxPower,
    long RangingInterval,
    bool TagToTagEnabled,
    long TagToTagInterval
);

public sealed record DioConfigEventDto(
    Guid Id,
    Guid RawEventId,
    Guid TagId,
    DateTime EventTimestamp,
    int Pin,
    string Direction,
    bool PeriodicReportEnabled,
    long PeriodicReportInterval,
    bool ReportOnChange,
    string LowPassFilterJson
);

public sealed record TagDioConfigSnapshotDto(
    Guid Id,
    Guid TagId,
    int Pin,
    Guid LastRawEventId,
    DateTime LastReportedAt,
    string Direction,
    bool PeriodicReportEnabled,
    long PeriodicReportInterval,
    bool ReportOnChange,
    string LowPassFilterJson
);

public sealed record I2cConfigEventDto(
    Guid Id,
    Guid RawEventId,
    Guid TagId,
    DateTime EventTimestamp,
    bool Enabled,
    int ClockSpeed,
    string DevicesJson
);

public sealed record TagI2cConfigSnapshotDto(
    Guid Id,
    Guid TagId,
    Guid LastRawEventId,
    DateTime LastReportedAt,
    bool Enabled,
    int ClockSpeed,
    string DevicesJson
);
