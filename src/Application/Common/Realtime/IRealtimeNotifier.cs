namespace Application.Common.Realtime;

public interface IRealtimeNotifier
{
    Task LocationUpdatedAsync(LocationUpdatedRealtimeDto payload, CancellationToken ct = default);
    Task BatteryUpdatedAsync(BatteryUpdatedRealtimeDto payload, CancellationToken ct = default);
    Task AlarmRaisedAsync(AlarmRaisedRealtimeDto payload, CancellationToken ct = default);
    Task AnchorStatusChangedAsync(AnchorStatusChangedRealtimeDto payload, CancellationToken ct = default);
    Task TagStatusChangedAsync(TagStatusChangedRealtimeDto payload, CancellationToken ct = default);
}

public sealed record LocationUpdatedRealtimeDto(
    Guid TagId,
    string TagExternalId,
    string? TagCode,
    Guid? UserId,
    decimal X,
    decimal Y,
    decimal Z,
    decimal Accuracy,
    decimal Confidence,
    DateTime EventTimestamp,
    int UsedAnchorCount
);

public sealed record BatteryUpdatedRealtimeDto(
    Guid TagId,
    string TagExternalId,
    string? TagCode,
    Guid? AnchorId,
    string? AnchorExternalId,
    int BatteryLevel,
    DateTime EventTimestamp
);

public sealed record AlarmRaisedRealtimeDto(
    Guid AlarmId,
    string AlarmType,
    string Severity,
    string Status,
    string Title,
    Guid? TagId,
    string? TagExternalId,
    Guid? PeerTagId,
    string? PeerTagExternalId,
    Guid? AnchorId,
    string? AnchorExternalId,
    Guid? UserId,
    DateTime StartedAt
);

public sealed record AnchorStatusChangedRealtimeDto(
    Guid AnchorId,
    string AnchorExternalId,
    string? AnchorCode,
    string Status,
    string PreviousStatus,
    string? Reason,
    DateTime EventTimestamp
);

public sealed record TagStatusChangedRealtimeDto(
    Guid TagId,
    string TagExternalId,
    string? TagCode,
    string Status,
    DateTime EventTimestamp
);