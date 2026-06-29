using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class FloorMapRepository : IFloorMapRepository
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public FloorMapRepository(AppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<FloorMap?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await ApplyScope(_db.FloorMaps)
            .Include(x => x.Files)
            .Include(x => x.Calibrations)
            .Include(x => x.AnchorPositions).ThenInclude(x => x.Anchor)
            .Include(x => x.Zones)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<FloorMap?> GetByCodeAsync(string code, CancellationToken ct = default)
    {
        return await ApplyScope(_db.FloorMaps)
            .FirstOrDefaultAsync(x => x.Code == code, ct);
    }

    public async Task<IReadOnlyList<FloorMap>> GetAllAsync(CancellationToken ct = default)
    {
        return await ApplyScope(_db.FloorMaps)
            .OrderBy(x => x.Code)
            .ToListAsync(ct);
    }

    public async Task AddAsync(FloorMap map, CancellationToken ct = default)
    {
        await _db.FloorMaps.AddAsync(map, ct);
    }

    public Task UpdateAsync(FloorMap map, CancellationToken ct = default)
    {
        _db.FloorMaps.Update(map);
        return Task.CompletedTask;
    }

    public async Task<FloorMapFile?> GetFileByIdAsync(Guid id, CancellationToken ct = default)
    {
        var maps = ApplyScope(_db.FloorMaps);
        return await _db.FloorMapFiles.FirstOrDefaultAsync(x => x.Id == id && maps.Any(m => m.Id == x.FloorMapId), ct);
    }

    public async Task AddFileAsync(FloorMapFile file, CancellationToken ct = default)
    {
        await _db.FloorMapFiles.AddAsync(file, ct);
    }

    public Task UpdateFileAsync(FloorMapFile file, CancellationToken ct = default)
    {
        _db.FloorMapFiles.Update(file);
        return Task.CompletedTask;
    }

    public async Task<FloorMapCalibration?> GetDefaultCalibrationAsync(Guid floorMapId, CancellationToken ct = default)
    {
        var maps = ApplyScope(_db.FloorMaps);
        return await _db.FloorMapCalibrations
            .FirstOrDefaultAsync(x =>
                x.FloorMapId == floorMapId &&
                maps.Any(m => m.Id == x.FloorMapId) &&
                x.IsActive &&
                x.IsDefault,
                ct);
    }

    public async Task<IReadOnlyList<FloorMapCalibration>> GetCalibrationsAsync(Guid floorMapId, CancellationToken ct = default)
    {
        var maps = ApplyScope(_db.FloorMaps);
        return await _db.FloorMapCalibrations
            .Where(x => x.FloorMapId == floorMapId && maps.Any(m => m.Id == x.FloorMapId))
            .OrderByDescending(x => x.IsDefault)
            .ThenBy(x => x.Name)
            .ToListAsync(ct);
    }

    public async Task<FloorMapCalibration?> GetCalibrationByIdAsync(Guid id, CancellationToken ct = default)
    {
        var maps = ApplyScope(_db.FloorMaps);
        return await _db.FloorMapCalibrations.FirstOrDefaultAsync(x => x.Id == id && maps.Any(m => m.Id == x.FloorMapId), ct);
    }

    public async Task AddCalibrationAsync(FloorMapCalibration calibration, CancellationToken ct = default)
    {
        await _db.FloorMapCalibrations.AddAsync(calibration, ct);
    }

    public Task UpdateCalibrationAsync(FloorMapCalibration calibration, CancellationToken ct = default)
    {
        _db.FloorMapCalibrations.Update(calibration);
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<AnchorMapPosition>> GetAnchorPositionsAsync(Guid floorMapId, CancellationToken ct = default)
    {
        var maps = ApplyScope(_db.FloorMaps);
        return await _db.AnchorMapPositions
            .Include(x => x.Anchor)
            .Where(x => x.FloorMapId == floorMapId && maps.Any(m => m.Id == x.FloorMapId))
            .OrderBy(x => x.Anchor.Code)
            .ToListAsync(ct);
    }

    public async Task<AnchorMapPosition?> GetAnchorPositionAsync(Guid floorMapId, Guid anchorId, CancellationToken ct = default)
    {
        var maps = ApplyScope(_db.FloorMaps);
        return await _db.AnchorMapPositions
            .Include(x => x.Anchor)
            .FirstOrDefaultAsync(x => x.FloorMapId == floorMapId && x.AnchorId == anchorId && maps.Any(m => m.Id == x.FloorMapId), ct);
    }

    public async Task AddAnchorPositionAsync(AnchorMapPosition position, CancellationToken ct = default)
    {
        await _db.AnchorMapPositions.AddAsync(position, ct);
    }

    public Task UpdateAnchorPositionAsync(AnchorMapPosition position, CancellationToken ct = default)
    {
        _db.AnchorMapPositions.Update(position);
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<FloorMapZone>> GetZonesAsync(Guid floorMapId, CancellationToken ct = default)
    {
        var maps = ApplyScope(_db.FloorMaps);
        return await _db.FloorMapZones
            .Where(x => x.FloorMapId == floorMapId && maps.Any(m => m.Id == x.FloorMapId))
            .OrderBy(x => x.Name)
            .ToListAsync(ct);
    }

    public async Task<FloorMapZone?> GetZoneByIdAsync(Guid zoneId, CancellationToken ct = default)
    {
        var maps = ApplyScope(_db.FloorMaps);
        return await _db.FloorMapZones.FirstOrDefaultAsync(x => x.Id == zoneId && maps.Any(m => m.Id == x.FloorMapId), ct);
    }

    public async Task AddZoneAsync(FloorMapZone zone, CancellationToken ct = default)
    {
        await _db.FloorMapZones.AddAsync(zone, ct);
    }

    public Task UpdateZoneAsync(FloorMapZone zone, CancellationToken ct = default)
    {
        _db.FloorMapZones.Update(zone);
        return Task.CompletedTask;
    }

    public async Task<FloorMap?> GetActiveMapByUsedAnchorExternalIdsAsync(
    IEnumerable<string> anchorExternalIds,
    CancellationToken ct = default)
    {
        var ids = anchorExternalIds
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct()
            .ToList();

        if (ids.Count == 0)
            return null;

        return await _db.AnchorMapPositions
            .Include(x => x.Anchor)
            .Include(x => x.FloorMap)
            .Where(x =>
                ids.Contains(x.Anchor.ExternalId) &&
                ApplyScope(_db.FloorMaps).Any(m => m.Id == x.FloorMapId) &&
                x.FloorMap.IsActive)
            .OrderBy(x => x.FloorMap.Code)
            .Select(x => x.FloorMap)
            .FirstOrDefaultAsync(ct);
    }

    public IQueryable<FloorMap> Query() => _db.FloorMaps.AsQueryable();

    private IQueryable<FloorMap> ApplyScope(IQueryable<FloorMap> query)
    {
        if (HasUnrestrictedScope())
            return query;

        var companyIds = _currentUser.GetCurrentUserCompanyIds();
        var branchIds = _currentUser.GetCurrentUserBranchIds();

        return query.Where(x =>
            x.CompanyId == null ||
            companyIds.Contains(x.CompanyId.Value) ||
            (x.BranchId.HasValue && branchIds.Contains(x.BranchId.Value)));
    }

    private bool HasUnrestrictedScope()
        => _currentUser.IsSystemUser() ||
           _currentUser.GetRoles().Any(x => x.Equals("super_admin", StringComparison.OrdinalIgnoreCase));
}
