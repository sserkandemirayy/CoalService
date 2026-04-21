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
