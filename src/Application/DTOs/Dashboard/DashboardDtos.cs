namespace Application.DTOs.Dashboard;

public sealed record DashboardSummaryDto(
    DashboardOperationSummaryDto Operation,
    DashboardAlarmSummaryDto Alarms,
    DashboardLocationSummaryDto Locations,
    DashboardAnchorSummaryDto Anchors,
    DashboardBatterySummaryDto Battery,
    DashboardEventSummaryDto Events,
    DashboardSystemHealthDto SystemHealth
);

public sealed record DashboardOperationSummaryDto(
    int TotalTags,
    int ActiveTags,
    int OnlineTags,
    int OfflineTags,
    int InactiveTags,
    int AssignedTags,
    int UnassignedTags,
    int PersonnelTags,
    int VehicleTags,
    int AssetTags
);

public sealed record DashboardAlarmSummaryDto(
    int ActiveAlarms,
    int AcknowledgedAlarms,
    int CriticalAlarms,
    int WarningAlarms,
    int TodayAlarms,
    int Last24HoursAlarms,
    int EmergencyButtonAlarms,
    int ProximityAlarms,
    int LowBatteryAlarms,
    int AnchorOfflineAlarms
);

public sealed record DashboardLocationSummaryDto(
    int CurrentlyLocatedTags,
    int SeenLast5Minutes,
    int SeenLast15Minutes,
    int LostLocationTags,
    int ActivePersonnel,
    int ActiveVehicles,
    int ActiveAssets
);

public sealed record DashboardAnchorSummaryDto(
    int TotalAnchors,
    int ActiveAnchors,
    int OnlineAnchors,
    int OfflineAnchors,
    int HeartbeatMissingAnchors
);

public sealed record DashboardBatterySummaryDto(
    int CriticalBatteryTags,
    int LowBatteryTags,
    decimal AverageBatteryLevel,
    int BatteryWarningsLast24Hours
);

public sealed record DashboardEventSummaryDto(
    int TodayRawEvents,
    int LastHourRawEvents,
    int ProcessedEvents,
    int FailedEvents,
    int PendingEvents,
    decimal SuccessRate
);

public sealed record DashboardSystemHealthDto(
    int Score,
    DateTime? LastEventAt
);

public sealed record RecentAlarmDto(
    Guid Id,
    string AlarmType,
    string Severity,
    string Status,
    string Title,
    Guid? TagId,
    string? TagCode,
    string? TagName,
    Guid? AnchorId,
    string? AnchorCode,
    string? AnchorName,
    Guid? UserId,
    string? UserFullName,
    DateTime StartedAt,
    DateTime? AcknowledgedAt,
    DateTime? EndedAt
);

public sealed record RecentEmergencyDto(
    Guid Id,
    Guid TagId,
    string TagCode,
    string? TagName,
    Guid? UserId,
    string? UserFullName,
    DateTime EventTimestamp
);

public sealed record RecentLocationDto(
    Guid Id,
    Guid TagId,
    string TagCode,
    string? TagName,
    string TagType,
    Guid? UserId,
    string? UserFullName,
    Guid? FloorMapId,
    Guid? FloorMapZoneId,
    decimal X,
    decimal Y,
    decimal Z,
    decimal Accuracy,
    decimal Confidence,
    int LastKnownAnchorCount,
    DateTime LastEventAt
);

public sealed record PersonnelPerformanceDashboardDto(
    int ActivePersonnelNow,
    int MovingPersonnelLast5Minutes,
    int SilentPersonnelLast15Minutes,
    int LostPersonnelLast30Minutes,
    int PersonnelWithNoMovementToday,
    IReadOnlyList<PersonnelActivityDto> MostActivePersonnelToday,
    IReadOnlyList<PersonnelActivityDto> LeastActivePersonnelToday,
    IReadOnlyList<SilentPersonnelDto> LongestSilentPersonnel,
    IReadOnlyList<SilentPersonnelDto> PersonnelWithAssignedTagButNoLocation
);

public sealed record PersonnelActivityDto(
    Guid? UserId,
    string? UserFullName,
    Guid TagId,
    string TagCode,
    string? TagName,
    int LocationEventCount,
    DateTime? LastSeenAt
);

public sealed record SilentPersonnelDto(
    Guid? UserId,
    string? UserFullName,
    Guid TagId,
    string TagCode,
    string? TagName,
    DateTime? LastSeenAt,
    int SilentMinutes
);

public sealed record PersonnelRisksDashboardDto(
    int ActiveEmergencyAlarms,
    int UnacknowledgedAlarms,
    int ProximityViolationsLast24Hours,
    int PersonnelInDangerZones,
    IReadOnlyList<RiskyPersonnelDto> MostAlarmProducingPersonnel,
    IReadOnlyList<RecentAlarmDto> UnacknowledgedAlarmList,
    IReadOnlyList<RecentEmergencyDto> RecentEmergencies,
    IReadOnlyList<ProximityViolationDto> RecentProximityViolations
);

public sealed record RiskyPersonnelDto(
    Guid? UserId,
    string? UserFullName,
    Guid? TagId,
    string? TagCode,
    string? TagName,
    int AlarmCount,
    DateTime? LastAlarmAt
);

public sealed record ProximityViolationDto(
    Guid Id,
    Guid TagId,
    string TagCode,
    string? TagName,
    Guid PeerTagId,
    string PeerTagCode,
    string? PeerTagName,
    decimal Distance,
    decimal Threshold,
    string Severity,
    DateTime EventTimestamp
);

public sealed record ZoneOccupancyDashboardDto(
    int KnownZoneCount,
    int UnknownZonePersonnelCount,
    IReadOnlyList<ZoneOccupancyDto> Zones,
    IReadOnlyList<ZoneOccupancyDto> DangerousZones,
    IReadOnlyList<ZoneRecentEntryDto> RecentZoneEntries
);

public sealed record ZoneOccupancyDto(
    Guid? FloorMapZoneId,
    string ZoneName,
    string ZoneType,
    int PersonnelCount,
    int VehicleCount,
    int AssetCount,
    int TotalCount
);

public sealed record ZoneRecentEntryDto(
    Guid CurrentLocationId,
    Guid TagId,
    string TagCode,
    string? TagName,
    string TagType,
    Guid? FloorMapZoneId,
    string? ZoneName,
    DateTime LastEventAt
);

public sealed record SystemHealthDashboardDto(
    int RtlsHealthScore,
    DateTime? LastEventAt,
    int EventsLastMinute,
    int EventsLast5Minutes,
    int FailedEventsLastHour,
    decimal ProcessingSuccessRateLastHour,
    int SilentAnchors,
    int OfflineAnchors,
    int HighCpuAnchors,
    int HighMemoryAnchors,
    int HighPacketLossAnchors,
    IReadOnlyList<ProblematicAnchorDto> ProblematicAnchors,
    IReadOnlyList<LiveFeedItemDto> RecentEvents
);

public sealed record ProblematicAnchorDto(
    Guid AnchorId,
    string AnchorCode,
    string? AnchorName,
    string Status,
    DateTime? LastHeartbeatAt,
    decimal? CpuUsage,
    decimal? MemoryUsage,
    decimal? PacketLossRate,
    DateTime? LastHealthAt,
    string Problem
);

public sealed record BatteryDisciplineDashboardDto(
    int CriticalBatteryTags,
    int LowBatteryTags,
    int LowBatteryPersonnelOnSite,
    decimal AverageBatteryLevel,
    IReadOnlyList<LowBatteryTagDto> LowestBatteryTags,
    IReadOnlyList<LowBatteryTagDto> LowBatteryTagsOnSite,
    IReadOnlyList<RecentBatteryAlertDto> RecentBatteryAlerts
);

public sealed record LowBatteryTagDto(
    Guid TagId,
    string TagCode,
    string? TagName,
    string TagType,
    Guid? UserId,
    string? UserFullName,
    int? BatteryLevel,
    DateTime? LastSeenAt
);

public sealed record RecentBatteryAlertDto(
    Guid Id,
    Guid TagId,
    string TagCode,
    string? TagName,
    Guid AnchorId,
    string AnchorCode,
    int BatteryLevel,
    DateTime EventTimestamp
);

public sealed record LiveFeedDashboardDto(
    IReadOnlyList<LiveFeedItemDto> Items
);

public sealed record LiveFeedItemDto(
    string Source,
    string EventType,
    string Title,
    string? Description,
    DateTime EventAt,
    Guid? RelatedId,
    Guid? TagId,
    string? TagCode,
    Guid? AnchorId,
    string? AnchorCode
);