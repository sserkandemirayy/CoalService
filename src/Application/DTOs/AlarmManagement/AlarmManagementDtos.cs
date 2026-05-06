namespace Application.DTOs.AlarmManagement;

public sealed record AlarmDto(
    Guid Id,
    Guid? RawEventId,
    string AlarmType,
    string Severity,
    string Status,
    Guid? TagId,
    Guid? PeerTagId,
    Guid? AnchorId,
    Guid? UserId,
    DateTime StartedAt,
    DateTime? EndedAt,
    DateTime? AcknowledgedAt,
    Guid? AcknowledgedBy,
    string Title,
    string? Description,
    string? DataJson
);