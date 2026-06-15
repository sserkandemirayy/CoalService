using Domain.Entities;

namespace Domain.Abstractions;

public interface IFloorMapRepository
{
    Task<FloorMap?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<FloorMap?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<IReadOnlyList<FloorMap>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(FloorMap map, CancellationToken ct = default);
    Task UpdateAsync(FloorMap map, CancellationToken ct = default);

    Task<FloorMapFile?> GetFileByIdAsync(Guid id, CancellationToken ct = default);
    Task AddFileAsync(FloorMapFile file, CancellationToken ct = default);
    Task UpdateFileAsync(FloorMapFile file, CancellationToken ct = default);

    Task<FloorMapCalibration?> GetDefaultCalibrationAsync(Guid floorMapId, CancellationToken ct = default);
    Task<IReadOnlyList<FloorMapCalibration>> GetCalibrationsAsync(Guid floorMapId, CancellationToken ct = default);
    Task<FloorMapCalibration?> GetCalibrationByIdAsync(Guid id, CancellationToken ct = default);
    Task AddCalibrationAsync(FloorMapCalibration calibration, CancellationToken ct = default);
    Task UpdateCalibrationAsync(FloorMapCalibration calibration, CancellationToken ct = default);

    Task<IReadOnlyList<AnchorMapPosition>> GetAnchorPositionsAsync(Guid floorMapId, CancellationToken ct = default);
    Task<AnchorMapPosition?> GetAnchorPositionAsync(Guid floorMapId, Guid anchorId, CancellationToken ct = default);
    Task AddAnchorPositionAsync(AnchorMapPosition position, CancellationToken ct = default);
    Task UpdateAnchorPositionAsync(AnchorMapPosition position, CancellationToken ct = default);

    Task<IReadOnlyList<FloorMapZone>> GetZonesAsync(Guid floorMapId, CancellationToken ct = default);
    Task<FloorMapZone?> GetZoneByIdAsync(Guid zoneId, CancellationToken ct = default);
    Task AddZoneAsync(FloorMapZone zone, CancellationToken ct = default);
    Task UpdateZoneAsync(FloorMapZone zone, CancellationToken ct = default);

    IQueryable<FloorMap> Query();
}