namespace Application.DTOs.DeviceManagement;

public sealed record TagDto(
    Guid Id,
    string ExternalId,
    string Code,
    string? Name,
    string? SerialNumber,
    string TagType,
    string Status,
    bool IsActive,
    int? BatteryLevel,
    DateTime? LastSeenAt,
    DateTime? LastEventAt,
    string? MetadataJson
);

public sealed record AnchorDto(
    Guid Id,
    string ExternalId,
    string Code,
    string? Name,
    string? IpAddress,
    string Status,
    bool IsActive,
    DateTime? LastHeartbeatAt,
    DateTime? LastStatusChangedAt,
    Guid? CompanyId,
    Guid? BranchId,
    string? MetadataJson
);

public sealed record TagAssignmentDto(
    Guid Id,
    Guid TagId,
    Guid UserId,
    DateTime AssignedAt,
    DateTime? UnassignedAt,
    bool IsPrimary,
    bool IsActiveAssignment
);