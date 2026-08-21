using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class MovementEventRepository
    : IMovementEventRepository
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public MovementEventRepository(
        AppDbContext db,
        ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    // ============================================================
    // WRITE
    // ============================================================

    public async Task AddAsync(
        MovementEvent movementEvent,
        CancellationToken ct = default)
    {
        await _db.MovementEvents.AddAsync(
            movementEvent,
            ct);
    }

    // ============================================================
    // INTERNAL MOVEMENT PROCESSING
    // ============================================================

    public async Task<MovementEvent?> GetLastByTagIdAsync(
        Guid tagId,
        CancellationToken ct = default)
    {
        if (tagId == Guid.Empty)
            return null;

        /*
         * IMPORTANT:
         *
         * Burada ApplyScope kullanılmıyor.
         *
         * Bu metod raporlama amacıyla değil,
         * event processing sırasında son movement
         * noktasını bulmak amacıyla kullanılıyor.
         *
         * Scope uygulanırsa background / integration
         * event processing sırasında önceki movement
         * kaydı görünmeyebilir.
         */

        return await _db.MovementEvents
            .AsNoTracking()
            .Where(x => x.TagId == tagId)
            .OrderByDescending(x => x.EventTimestamp)
            .ThenByDescending(x => x.Id)
            .FirstOrDefaultAsync(ct);
    }

    // ============================================================
    // EXISTING USER MOVEMENT HISTORY
    // ============================================================

    public async Task<(
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
            CancellationToken ct = default)
    {
        return await GetPersonMovementReportAsync(
            userId,
            from,
            to,
            companyId,
            branchId,
            floorMapId,
            floorMapZoneId,
            page,
            pageSize,
            ct);
    }

    // ============================================================
    // PERSON MOVEMENT REPORT
    // ============================================================

    public async Task<(
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
            CancellationToken ct = default)
    {
        from = EnsureUtc(from);
        to = EnsureUtc(to);

        page = Math.Max(
            1,
            page);

        pageSize = Math.Clamp(
            pageSize,
            1,
            5000);

        var query =
            ApplyScope(
                _db.MovementEvents
                    .AsNoTracking())
            .Where(x =>
                x.UserId == userId &&
                x.EventTimestamp >= from &&
                x.EventTimestamp <= to);

        if (companyId.HasValue)
        {
            query = query.Where(
                x => x.CompanyId ==
                     companyId.Value);
        }

        if (branchId.HasValue)
        {
            query = query.Where(
                x => x.BranchId ==
                     branchId.Value);
        }

        if (floorMapId.HasValue)
        {
            query = query.Where(
                x => x.FloorMapId ==
                     floorMapId.Value);
        }

        if (floorMapZoneId.HasValue)
        {
            query = query.Where(
                x => x.FloorMapZoneId ==
                     floorMapZoneId.Value);
        }

        var total =
            await query.LongCountAsync(ct);

        var items =
            await query
                .OrderBy(x =>
                    x.EventTimestamp)
                .ThenBy(x =>
                    x.Id)
                .Skip(
                    (page - 1) *
                    pageSize)
                .Take(
                    pageSize)
                .ToListAsync(ct);

        return (
            items,
            total);
    }

    // ============================================================
    // PLAYBACK
    // ============================================================

    public async Task<IReadOnlyList<MovementEvent>>
        GetPlaybackAsync(
            Guid userId,
            DateTime from,
            DateTime to,
            Guid? companyId,
            Guid? branchId,
            Guid? floorMapId,
            int maxPoints,
            CancellationToken ct = default)
    {
        from = EnsureUtc(from);
        to = EnsureUtc(to);

        maxPoints = Math.Clamp(
            maxPoints,
            1,
            100000);

        var query =
            ApplyScope(
                _db.MovementEvents
                    .AsNoTracking())
            .Where(x =>
                x.UserId == userId &&
                x.EventTimestamp >= from &&
                x.EventTimestamp <= to);

        if (companyId.HasValue)
        {
            query = query.Where(
                x => x.CompanyId ==
                     companyId.Value);
        }

        if (branchId.HasValue)
        {
            query = query.Where(
                x => x.BranchId ==
                     branchId.Value);
        }

        if (floorMapId.HasValue)
        {
            query = query.Where(
                x => x.FloorMapId ==
                     floorMapId.Value);
        }

        return await query
            .OrderBy(x =>
                x.EventTimestamp)
            .ThenBy(x =>
                x.Id)
            .Take(
                maxPoints)
            .ToListAsync(ct);
    }

    // ============================================================
    // 3D HEATMAP
    // ============================================================

    public async Task<IReadOnlyList<MovementHeatMapBucket>>
        GetHeatMapAsync(
            Guid floorMapId,
            DateTime from,
            DateTime to,
            Guid? userId,
            Guid? companyId,
            Guid? branchId,
            Guid? floorMapZoneId,
            decimal gridSize,
            CancellationToken ct = default)
    {
        from = EnsureUtc(from);
        to = EnsureUtc(to);

        if (floorMapId == Guid.Empty)
        {
            throw new ArgumentException(
                "FloorMapId is required.",
                nameof(floorMapId));
        }

        if (gridSize <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(gridSize),
                "Grid size must be greater than zero.");
        }

        var query =
            ApplyScope(
                _db.MovementEvents
                    .AsNoTracking())
            .Where(x =>
                x.FloorMapId == floorMapId &&
                x.EventTimestamp >= from &&
                x.EventTimestamp <= to);

        if (userId.HasValue)
        {
            query = query.Where(
                x => x.UserId ==
                     userId.Value);
        }

        if (companyId.HasValue)
        {
            query = query.Where(
                x => x.CompanyId ==
                     companyId.Value);
        }

        if (branchId.HasValue)
        {
            query = query.Where(
                x => x.BranchId ==
                     branchId.Value);
        }

        if (floorMapZoneId.HasValue)
        {
            query = query.Where(
                x => x.FloorMapZoneId ==
                     floorMapZoneId.Value);
        }

        /*
         * ========================================================
         * 3D VOXEL HEATMAP
         * ========================================================
         *
         * gridSize = 1 ise:
         *
         * Her voxel:
         *
         * 1m X 1m X 1m
         *
         * olacaktır.
         *
         * Örnek:
         *
         * Point A:
         * X = 12.10
         * Y = 8.20
         * Z = 1.30
         *
         * Point B:
         * X = 12.80
         * Y = 8.90
         * Z = 1.70
         *
         * aynı voxel içerisinde yer alır.
         *
         * Fakat:
         *
         * X = 12.80
         * Y = 8.90
         * Z = 2.20
         *
         * farklı bir Z voxel'ına düşer.
         */

        var grouped =
            await query
                .GroupBy(x => new
                {
                    XBucket =
                        Math.Floor(
                            x.X / gridSize),

                    YBucket =
                        Math.Floor(
                            x.Y / gridSize),

                    ZBucket =
                        Math.Floor(
                            x.Z / gridSize)
                })
                .Select(g => new
                {
                    g.Key.XBucket,
                    g.Key.YBucket,
                    g.Key.ZBucket,

                    Count =
                        g.LongCount()
                })
                .OrderByDescending(x =>
                    x.Count)
                .ToListAsync(ct);

        /*
         * API'ye voxel'in başlangıç noktası yerine
         * merkez koordinatını döndürüyoruz.
         *
         * Örnek:
         *
         * voxel:
         * X = 12 -> 13
         *
         * API:
         * X = 12.5
         *
         * döndürür.
         *
         * Bu frontend üzerinde 3D cube / sphere
         * çizimini kolaylaştırır.
         */

        var halfGrid =
            gridSize / 2m;

        return grouped
            .Select(x =>
                new MovementHeatMapBucket(
                    (x.XBucket *
                     gridSize) +
                    halfGrid,

                    (x.YBucket *
                     gridSize) +
                    halfGrid,

                    (x.ZBucket *
                     gridSize) +
                    halfGrid,

                    x.Count))
            .ToList();
    }

    // ============================================================
    // QUERY
    // ============================================================

    public IQueryable<MovementEvent> Query()
    {
        return _db.MovementEvents
            .AsQueryable();
    }

    // ============================================================
    // SECURITY SCOPE
    // ============================================================

    private IQueryable<MovementEvent> ApplyScope(
        IQueryable<MovementEvent> query)
    {
        if (HasUnrestrictedScope())
            return query;

        var currentUserId =
            _currentUser
                .GetCurrentUserId();

        var companyIds =
            _currentUser
                .GetCurrentUserCompanyIds();

        var branchIds =
            _currentUser
                .GetCurrentUserBranchIds();

        return query.Where(x =>

            // Kullanıcı kendi geçmişini görebilir.
            x.UserId ==
            currentUserId ||

            // Kullanıcının bağlı olduğu şirket.
            (x.CompanyId.HasValue &&
             companyIds.Contains(
                 x.CompanyId.Value)) ||

            // Kullanıcının bağlı olduğu şube.
            (x.BranchId.HasValue &&
             branchIds.Contains(
                 x.BranchId.Value)));
    }

    // ============================================================
    // UNRESTRICTED SCOPE
    // ============================================================

    private bool HasUnrestrictedScope()
    {
        return
            _currentUser.IsSystemUser() ||

            _currentUser
                .GetRoles()
                .Any(x =>
                    x.Equals(
                        "super_admin",
                        StringComparison.OrdinalIgnoreCase));
    }

    // ============================================================
    // UTC
    // ============================================================

    private static DateTime EnsureUtc(
        DateTime value)
    {
        if (value.Kind ==
            DateTimeKind.Utc)
        {
            return value;
        }

        if (value.Kind ==
            DateTimeKind.Local)
        {
            return value
                .ToUniversalTime();
        }

        return DateTime.SpecifyKind(
            value,
            DateTimeKind.Utc);
    }
}