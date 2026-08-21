namespace Application.DTOs.Movement;

// ============================================================
// PERSON MOVEMENT REPORT
// ============================================================

public sealed record PersonMovementReportItemDto(
    Guid Id,
    DateTime EventTimestamp,

    Guid RawEventId,

    Guid TagId,
    string TagExternalId,
    string TagCode,
    string TagType,

    Guid? UserId,
    string? UserFullName,
    string? UserCode,

    Guid? CompanyId,
    Guid? BranchId,

    Guid? FloorMapId,
    Guid? FloorMapZoneId,

    decimal X,
    decimal Y,
    decimal Z,

    decimal Accuracy,
    decimal Confidence,

    string RecordReason
);

public sealed record PersonMovementReportDto(
    Guid UserId,
    string? UserFullName,
    string? UserCode,

    DateTime From,
    DateTime To,

    long Total,
    int Page,
    int PageSize,
    int TotalPages,

    IReadOnlyList<PersonMovementReportItemDto> Items
);

// ============================================================
// PLAYBACK
// ============================================================

public sealed record MovementPlaybackPointDto(
    Guid Id,
    DateTime Timestamp,

    decimal X,
    decimal Y,
    decimal Z,

    decimal Accuracy,
    decimal Confidence,

    Guid? FloorMapZoneId,

    string RecordReason
);

public sealed record MovementPlaybackSegmentDto(
    Guid? FloorMapId,

    DateTime StartedAt,
    DateTime EndedAt,

    int PointCount,

    IReadOnlyList<MovementPlaybackPointDto> Points
);

public sealed record MovementPlaybackDto(
    Guid UserId,

    string? UserFullName,
    string? UserCode,

    DateTime From,
    DateTime To,

    int PointCount,

    bool IsTruncated,

    IReadOnlyList<MovementPlaybackSegmentDto> Segments
);

// ============================================================
// 3D HEATMAP
// ============================================================

/// <summary>
/// 3D heatmap voxel merkezini temsil eder.
///
/// X / Y / Z:
/// voxel'in merkez koordinatıdır.
///
/// Count:
/// ilgili voxel içerisine düşen movement point sayısıdır.
///
/// Intensity:
/// 0 - 1 arasında normalize edilmiş yoğunluktur.
/// </summary>
public sealed record MovementHeatMapCellDto(
    decimal X,
    decimal Y,
    decimal Z,

    long Count,

    decimal Intensity
);

public sealed record MovementHeatMapDto(
    Guid FloorMapId,

    DateTime From,
    DateTime To,

    Guid? UserId,
    Guid? CompanyId,
    Guid? BranchId,
    Guid? FloorMapZoneId,

    /// <summary>
    /// 3D voxel edge size.
    ///
    /// Örnek:
    /// GridSize = 1
    /// => 1m x 1m x 1m voxel.
    /// </summary>
    decimal GridSize,

    long TotalPointCount,
    long MaxCount,

    IReadOnlyList<MovementHeatMapCellDto> Cells
);