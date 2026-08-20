using Domain.Entities;

namespace Domain.Abstractions;

public sealed record MovementHeatMapBucket(
    decimal X,
    decimal Y,
    long Count
);

public interface IMovementEventRepository
{
    // ============================================================
    // WRITE
    // ============================================================

    Task AddAsync(
        MovementEvent movementEvent,
        CancellationToken ct = default);

    // ============================================================
    // INTERNAL MOVEMENT PROCESSING
    // ============================================================

    /// <summary>
    /// Tag için kaydedilmiş en son movement noktasını getirir.
    ///
    /// Event processing sırasında yeni movement kaydının yazılıp
    /// yazılmayacağına karar vermek için kullanılır.
    ///
    /// Security/report scope uygulanmaz.
    /// </summary>
    Task<MovementEvent?> GetLastByTagIdAsync(
        Guid tagId,
        CancellationToken ct = default);

    // ============================================================
    // EXISTING USER MOVEMENT HISTORY
    // ============================================================

    Task<(
        IReadOnlyList<MovementEvent> Items,
        long Total)>
        GetUserMovementHistoryAsync(
            Guid userId,
            DateTime from,
            DateTime to,
            Guid? companyId,
            Guid? branchId,
            Guid? floorMapId,
            Guid? floorMapZoneId,
            int page,
            int pageSize,
            CancellationToken ct = default);

    // ============================================================
    // PERSON MOVEMENT REPORT
    // ============================================================

    Task<(
        IReadOnlyList<MovementEvent> Items,
        long Total)>
        GetPersonMovementReportAsync(
            Guid userId,
            DateTime from,
            DateTime to,
            Guid? companyId,
            Guid? branchId,
            Guid? floorMapId,
            Guid? floorMapZoneId,
            int page,
            int pageSize,
            CancellationToken ct = default);

    // ============================================================
    // PLAYBACK
    // ============================================================

    Task<IReadOnlyList<MovementEvent>>
        GetPlaybackAsync(
            Guid userId,
            DateTime from,
            DateTime to,
            Guid? companyId,
            Guid? branchId,
            Guid? floorMapId,
            int maxPoints,
            CancellationToken ct = default);

    // ============================================================
    // HEATMAP
    // ============================================================

    Task<IReadOnlyList<MovementHeatMapBucket>>
        GetHeatMapAsync(
            Guid floorMapId,
            DateTime from,
            DateTime to,
            Guid? userId,
            Guid? companyId,
            Guid? branchId,
            Guid? floorMapZoneId,
            decimal gridSize,
            CancellationToken ct = default);

    // ============================================================
    // QUERY
    // ============================================================

    IQueryable<MovementEvent> Query();
}