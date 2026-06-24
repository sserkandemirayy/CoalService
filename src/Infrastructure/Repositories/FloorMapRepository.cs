using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class FloorMapRepository : IFloorMapRepository
{
    private readonly AppDbContext _db;

    public FloorMapRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<FloorMap?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _db.FloorMaps
            .Include(x => x.Files)
            .Include(x => x.Calibrations)
            .Include(x => x.AnchorPositions).ThenInclude(x => x.Anchor)
            .Include(x => x.Zones)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<FloorMap?> GetByCodeAsync(string code, CancellationToken ct = default)
    {
        return await _db.FloorMaps
            .FirstOrDefaultAsync(x => x.Code == code, ct);
    }

    public async Task<IReadOnlyList<FloorMap>> GetAllAsync(CancellationToken ct = default)
    {
        return await _db.FloorMaps
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
        return await _db.FloorMapFiles.FirstOrDefaultAsync(x => x.Id == id, ct);
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
        return await _db.FloorMapCalibrations
            .FirstOrDefaultAsync(x =>
                x.FloorMapId == floorMapId &&
                x.IsActive &&
                x.IsDefault,
                ct);
    }

    public async Task<IReadOnlyList<FloorMapCalibration>> GetCalibrationsAsync(Guid floorMapId, CancellationToken ct = default)
    {
        return await _db.FloorMapCalibrations
            .Where(x => x.FloorMapId == floorMapId)
            .OrderByDescending(x => x.IsDefault)
            .ThenBy(x => x.Name)
            .ToListAsync(ct);
    }

    public async Task<FloorMapCalibration?> GetCalibrationByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _db.FloorMapCalibrations.FirstOrDefaultAsync(x => x.Id == id, ct);
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
        return await _db.AnchorMapPositions
            .Include(x => x.Anchor)
            .Where(x => x.FloorMapId == floorMapId)
            .OrderBy(x => x.Anchor.Code)
            .ToListAsync(ct);
    }

    public async Task<AnchorMapPosition?> GetAnchorPositionAsync(Guid floorMapId, Guid anchorId, CancellationToken ct = default)
    {
        return await _db.AnchorMapPositions
            .Include(x => x.Anchor)
            .FirstOrDefaultAsync(x => x.FloorMapId == floorMapId && x.AnchorId == anchorId, ct);
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
        return await _db.FloorMapZones
            .Where(x => x.FloorMapId == floorMapId)
            .OrderBy(x => x.Name)
            .ToListAsync(ct);
    }

    public async Task<FloorMapZone?> GetZoneByIdAsync(Guid zoneId, CancellationToken ct = default)
    {
        return await _db.FloorMapZones.FirstOrDefaultAsync(x => x.Id == zoneId, ct);
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
                x.FloorMap.IsActive)
            .OrderBy(x => x.FloorMap.Code)
            .Select(x => x.FloorMap)
            .FirstOrDefaultAsync(ct);
    }

    public IQueryable<FloorMap> Query() => _db.FloorMaps.AsQueryable();
}