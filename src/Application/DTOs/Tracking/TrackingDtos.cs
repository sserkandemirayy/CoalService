using Domain.Enums;

namespace Application.DTOs.Tracking;

public sealed record CurrentLocationDto(
    Guid Id,
    Guid TagId,
    string TagExternalId,
    string TagCode,
    string TagType,
    Guid? UserId,
    string? UserFullName,
    string? UserIdentifier,
    Guid? FloorMapId,
    Guid? FloorMapZoneId,
    decimal X,
    decimal Y,
    decimal Z,
    decimal Accuracy,
    decimal Confidence,
    DateTime LastEventAt,
    Guid LastRawEventId,
    int LastKnownAnchorCount
);

public sealed record TagHistoryPointDto(
    Guid LocationEventId,
    DateTime EventTimestamp,
    decimal X,
    decimal Y,
    decimal Z,
    decimal Accuracy,
    decimal Confidence
);

public sealed record TrackingDashboardDto(
    int TotalTags,
    int OnlineTags,
    int ActiveAlarms,
    int OnlineAnchors,
    int OfflineAnchors,
    int ErrorAnchors
);