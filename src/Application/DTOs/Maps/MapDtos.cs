using Domain.Enums;

namespace Application.DTOs.Maps;


public sealed record FloorMapDto(
    Guid Id,
    string Code,
    string Name,
    string? Description,
    Guid? CompanyId,
    Guid? BranchId,
    decimal Width,
    decimal Height,
    string Unit,
    bool IsActive
);

public sealed record CreateFloorMapDto(
    string Code,
    string Name,
    string? Description,
    Guid? CompanyId,
    Guid? BranchId,
    decimal Width,
    decimal Height,
    string? Unit
);

public sealed record UpdateFloorMapDto(
    string Code,
    string Name,
    string? Description,
    Guid? CompanyId,
    Guid? BranchId,
    decimal Width,
    decimal Height,
    string? Unit,
    bool IsActive
);

public sealed record FloorMapFileDto(
    Guid Id,
    Guid FloorMapId,
    string FileType,
    string OriginalFileName,
    string StoredFileName,
    string ContentType,
    string StoragePath,
    long FileSize,
    int Version,
    bool IsActive
);

public sealed record CreateCalibrationDto(
    string Name,
    decimal SourceOriginX,
    decimal SourceOriginY,
    decimal SourceOriginZ,
    decimal MapOriginX,
    decimal MapOriginY,
    decimal MapOriginZ,
    decimal ScaleX,
    decimal ScaleY,
    decimal ScaleZ,
    decimal RotationDegrees,
    bool IsDefault,
    bool IsActive
);

public sealed record FloorMapCalibrationDto(
    Guid Id,
    Guid FloorMapId,
    string Name,
    decimal SourceOriginX,
    decimal SourceOriginY,
    decimal SourceOriginZ,
    decimal MapOriginX,
    decimal MapOriginY,
    decimal MapOriginZ,
    decimal ScaleX,
    decimal ScaleY,
    decimal ScaleZ,
    decimal RotationDegrees,
    bool IsDefault,
    bool IsActive
);

public sealed record TransformCoordinateRequestDto(
    decimal X,
    decimal Y,
    decimal Z
);

public sealed record TransformCoordinateResponseDto(
    decimal SourceX,
    decimal SourceY,
    decimal SourceZ,
    decimal MapX,
    decimal MapY,
    decimal MapZ
);

public sealed record AnchorMapPositionDto(
    Guid Id,
    Guid FloorMapId,
    Guid AnchorId,
    string? AnchorCode,
    string? AnchorName,
    decimal X,
    decimal Y,
    decimal Z,
    decimal? Rotation,
    string? MetadataJson
);

public sealed record UpsertAnchorMapPositionDto(
    Guid AnchorId,
    decimal X,
    decimal Y,
    decimal Z,
    decimal? Rotation,
    string? MetadataJson
);

public sealed record FloorMapZoneDto(
    Guid Id,
    Guid FloorMapId,
    string Name,
    string ZoneType,
    string? Color,
    string GeometryJson,
    bool IsActive
);

public sealed record CreateFloorMapZoneDto(
    string Name,
    FloorMapZoneType ZoneType,
    string? Color,
    string GeometryJson
);

public sealed record UpdateFloorMapZoneDto(
    string Name,
    FloorMapZoneType ZoneType,
    string? Color,
    string GeometryJson,
    bool IsActive
);
