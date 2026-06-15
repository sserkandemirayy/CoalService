using Application.Common.Maps;
using Application.DTOs.Maps;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[Authorize]
public class MapsController : BaseController
{
    private readonly IFloorMapRepository _maps;
    private readonly IAnchorRepository _anchors;
    private readonly IMapFileStorageService _storage;
    private readonly IMapCoordinateTransformer _transformer;
    private readonly IUnitOfWork _uow;

    public MapsController(
        IFloorMapRepository maps,
        IAnchorRepository anchors,
        IMapFileStorageService storage,
        IMapCoordinateTransformer transformer,
        IUnitOfWork uow)
    {
        _maps = maps;
        _anchors = anchors;
        _storage = storage;
        _transformer = transformer;
        _uow = uow;
    }

    [HttpGet]
    [Authorize(Policy = "ViewDevices")]
    public async Task<IActionResult> GetMaps(CancellationToken ct)
    {
        var maps = await _maps.GetAllAsync(ct);

        return Ok(maps.Select(x => new FloorMapDto(
            x.Id,
            x.Code,
            x.Name,
            x.Description,
            x.CompanyId,
            x.BranchId,
            x.Width,
            x.Height,
            x.Unit,
            x.IsActive
        )));
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "ViewDevices")]
    public async Task<IActionResult> GetMap(Guid id, CancellationToken ct)
    {
        var map = await _maps.GetByIdAsync(id, ct);

        if (map is null)
            return NotFound(new { error = "Map not found." });

        return Ok(new FloorMapDto(
            map.Id,
            map.Code,
            map.Name,
            map.Description,
            map.CompanyId,
            map.BranchId,
            map.Width,
            map.Height,
            map.Unit,
            map.IsActive
        ));
    }

    [HttpPost]
    [Authorize(Policy = "ManageDeviceConfigs")]
    public async Task<IActionResult> CreateMap([FromBody] CreateFloorMapDto dto, CancellationToken ct)
    {
        var existing = await _maps.GetByCodeAsync(dto.Code, ct);
        if (existing is not null)
            return Conflict(new { error = "Map code already exists." });

        var map = FloorMap.Create(
            dto.Code,
            dto.Name,
            dto.Description,
            dto.CompanyId,
            dto.BranchId,
            dto.Width,
            dto.Height,
            dto.Unit
        );

        await _maps.AddAsync(map, ct);

        var calibration = FloorMapCalibration.CreateIdentity(map.Id);
        await _maps.AddCalibrationAsync(calibration, ct);

        await _uow.SaveChangesAsync(ct);

        return Ok(new { id = map.Id });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "ManageDeviceConfigs")]
    public async Task<IActionResult> UpdateMap(Guid id, [FromBody] UpdateFloorMapDto dto, CancellationToken ct)
    {
        var map = await _maps.GetByIdAsync(id, ct);

        if (map is null)
            return NotFound(new { error = "Map not found." });

        map.Update(
            dto.Code,
            dto.Name,
            dto.Description,
            dto.CompanyId,
            dto.BranchId,
            dto.Width,
            dto.Height,
            dto.Unit
        );

        if (dto.IsActive)
            map.Activate();
        else
            map.Deactivate();

        await _maps.UpdateAsync(map, ct);
        await _uow.SaveChangesAsync(ct);

        return Ok(new { status = "updated" });
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "ManageDeviceConfigs")]
    public async Task<IActionResult> DeleteMap(Guid id, CancellationToken ct)
    {
        var map = await _maps.GetByIdAsync(id, ct);

        if (map is null)
            return NotFound(new { error = "Map not found." });

        map.SoftDelete(CurrentUserId);

        await _maps.UpdateAsync(map, ct);
        await _uow.SaveChangesAsync(ct);

        return NoContent();
    }

    [HttpGet("{id:guid}/files")]
    [Authorize(Policy = "ViewDevices")]
    public async Task<IActionResult> GetFiles(Guid id, CancellationToken ct)
    {
        var map = await _maps.GetByIdAsync(id, ct);

        if (map is null)
            return NotFound(new { error = "Map not found." });

        return Ok(map.Files.Select(x => new FloorMapFileDto(
            x.Id,
            x.FloorMapId,
            x.FileType.ToString(),
            x.OriginalFileName,
            x.StoredFileName,
            x.ContentType,
            x.StoragePath,
            x.FileSize,
            x.Version,
            x.IsActive
        )));
    }

    [HttpPost("{id:guid}/files")]
    [Authorize(Policy = "ManageDeviceConfigs")]
    public async Task<IActionResult> UploadFile(
                Guid id,
                [FromForm] UploadFloorMapFileRequest request,
                CancellationToken ct)
    {
        var map = await _maps.GetByIdAsync(id, ct);

        if (map is null)
            return NotFound(new { error = "Map not found." });

        if (request.File is null || request.File.Length == 0)
            return BadRequest(new { error = "File is required." });

        await using var stream = request.File.OpenReadStream();

        var stored = await _storage.SaveAsync(
            id,
            request.File.FileName,
            request.File.ContentType,
            stream,
            ct
        );

        var mapFile = FloorMapFile.Create(
            id,
            request.FileType,
            request.File.FileName,
            stored.StoredFileName,
            request.File.ContentType,
            stored.StoragePath,
            stored.FileSize,
            request.Version <= 0 ? 1 : request.Version
        );

        await _maps.AddFileAsync(mapFile, ct);
        await _uow.SaveChangesAsync(ct);

        return Ok(new { id = mapFile.Id });
    }

    [HttpDelete("files/{fileId:guid}")]
    [Authorize(Policy = "ManageDeviceConfigs")]
    public async Task<IActionResult> DeleteFile(Guid fileId, CancellationToken ct)
    {
        var file = await _maps.GetFileByIdAsync(fileId, ct);

        if (file is null)
            return NotFound(new { error = "File not found." });

        file.Deactivate();
        file.SoftDelete(CurrentUserId);

        await _maps.UpdateFileAsync(file, ct);
        await _storage.DeleteIfExistsAsync(file.StoragePath, ct);
        await _uow.SaveChangesAsync(ct);

        return NoContent();
    }

    [HttpGet("{id:guid}/calibrations")]
    [Authorize(Policy = "ViewDevices")]
    public async Task<IActionResult> GetCalibrations(Guid id, CancellationToken ct)
    {
        var calibrations = await _maps.GetCalibrationsAsync(id, ct);

        return Ok(calibrations.Select(ToCalibrationDto));
    }

    [HttpPost("{id:guid}/calibrations")]
    [Authorize(Policy = "ManageDeviceConfigs")]
    public async Task<IActionResult> CreateCalibration(Guid id, [FromBody] CreateCalibrationDto dto, CancellationToken ct)
    {
        var map = await _maps.GetByIdAsync(id, ct);

        if (map is null)
            return NotFound(new { error = "Map not found." });

        var calibration = FloorMapCalibration.Create(
            id,
            dto.Name,
            dto.SourceOriginX,
            dto.SourceOriginY,
            dto.SourceOriginZ,
            dto.MapOriginX,
            dto.MapOriginY,
            dto.MapOriginZ,
            dto.ScaleX,
            dto.ScaleY,
            dto.ScaleZ,
            dto.RotationDegrees,
            dto.IsDefault
        );

        calibration.Update(
            dto.Name,
            dto.SourceOriginX,
            dto.SourceOriginY,
            dto.SourceOriginZ,
            dto.MapOriginX,
            dto.MapOriginY,
            dto.MapOriginZ,
            dto.ScaleX,
            dto.ScaleY,
            dto.ScaleZ,
            dto.RotationDegrees,
            dto.IsDefault,
            dto.IsActive
        );

        await _maps.AddCalibrationAsync(calibration, ct);
        await _uow.SaveChangesAsync(ct);

        return Ok(new { id = calibration.Id });
    }

    [HttpPut("calibrations/{calibrationId:guid}")]
    [Authorize(Policy = "ManageDeviceConfigs")]
    public async Task<IActionResult> UpdateCalibration(
        Guid calibrationId,
        [FromBody] CreateCalibrationDto dto,
        CancellationToken ct)
    {
        var calibration = await _maps.GetCalibrationByIdAsync(calibrationId, ct);

        if (calibration is null)
            return NotFound(new { error = "Calibration not found." });

        calibration.Update(
            dto.Name,
            dto.SourceOriginX,
            dto.SourceOriginY,
            dto.SourceOriginZ,
            dto.MapOriginX,
            dto.MapOriginY,
            dto.MapOriginZ,
            dto.ScaleX,
            dto.ScaleY,
            dto.ScaleZ,
            dto.RotationDegrees,
            dto.IsDefault,
            dto.IsActive
        );

        await _maps.UpdateCalibrationAsync(calibration, ct);
        await _uow.SaveChangesAsync(ct);

        return Ok(new { status = "updated" });
    }

    [HttpPost("{id:guid}/transform")]
    [Authorize(Policy = "ViewDevices")]
    public async Task<IActionResult> TransformCoordinate(
        Guid id,
        [FromBody] TransformCoordinateRequestDto dto,
        CancellationToken ct)
    {
        var calibration = await _maps.GetDefaultCalibrationAsync(id, ct);

        if (calibration is null)
            return NotFound(new { error = "Default calibration not found." });

        var mapped = _transformer.TransformToMap(
            calibration,
            dto.X,
            dto.Y,
            dto.Z
        );

        return Ok(new TransformCoordinateResponseDto(
            dto.X,
            dto.Y,
            dto.Z,
            mapped.X,
            mapped.Y,
            mapped.Z
        ));
    }

    [HttpGet("{id:guid}/anchors")]
    [Authorize(Policy = "ViewDevices")]
    public async Task<IActionResult> GetAnchorPositions(Guid id, CancellationToken ct)
    {
        var positions = await _maps.GetAnchorPositionsAsync(id, ct);

        return Ok(positions.Select(x => new AnchorMapPositionDto(
            x.Id,
            x.FloorMapId,
            x.AnchorId,
            x.Anchor.Code,
            x.Anchor.Name,
            x.X,
            x.Y,
            x.Z,
            x.Rotation,
            x.MetadataJson
        )));
    }

    [HttpPost("{id:guid}/anchors")]
    [Authorize(Policy = "ManageDeviceConfigs")]
    public async Task<IActionResult> UpsertAnchorPosition(
        Guid id,
        [FromBody] UpsertAnchorMapPositionDto dto,
        CancellationToken ct)
    {
        var map = await _maps.GetByIdAsync(id, ct);
        if (map is null)
            return NotFound(new { error = "Map not found." });

        var anchor = await _anchors.GetByIdAsync(dto.AnchorId, ct);
        if (anchor is null)
            return NotFound(new { error = "Anchor not found." });

        var existing = await _maps.GetAnchorPositionAsync(id, dto.AnchorId, ct);

        if (existing is null)
        {
            var position = AnchorMapPosition.Create(
                id,
                dto.AnchorId,
                dto.X,
                dto.Y,
                dto.Z,
                dto.Rotation,
                dto.MetadataJson
            );

            await _maps.AddAnchorPositionAsync(position, ct);
            await _uow.SaveChangesAsync(ct);

            return Ok(new { id = position.Id, status = "created" });
        }

        existing.Update(
            dto.X,
            dto.Y,
            dto.Z,
            dto.Rotation,
            dto.MetadataJson
        );

        await _maps.UpdateAnchorPositionAsync(existing, ct);
        await _uow.SaveChangesAsync(ct);

        return Ok(new { id = existing.Id, status = "updated" });
    }

    [HttpDelete("{id:guid}/anchors/{anchorId:guid}")]
    [Authorize(Policy = "ManageDeviceConfigs")]
    public async Task<IActionResult> DeleteAnchorPosition(Guid id, Guid anchorId, CancellationToken ct)
    {
        var position = await _maps.GetAnchorPositionAsync(id, anchorId, ct);

        if (position is null)
            return NotFound(new { error = "Anchor position not found." });

        position.SoftDelete(CurrentUserId);

        await _maps.UpdateAnchorPositionAsync(position, ct);
        await _uow.SaveChangesAsync(ct);

        return NoContent();
    }

    [HttpGet("{id:guid}/zones")]
    [Authorize(Policy = "ViewDevices")]
    public async Task<IActionResult> GetZones(Guid id, CancellationToken ct)
    {
        var zones = await _maps.GetZonesAsync(id, ct);

        return Ok(zones.Select(ToZoneDto));
    }

    [HttpPost("{id:guid}/zones")]
    [Authorize(Policy = "ManageDeviceConfigs")]
    public async Task<IActionResult> CreateZone(Guid id, [FromBody] CreateFloorMapZoneDto dto, CancellationToken ct)
    {
        var map = await _maps.GetByIdAsync(id, ct);

        if (map is null)
            return NotFound(new { error = "Map not found." });

        var zone = FloorMapZone.Create(
            id,
            dto.Name,
            dto.ZoneType,
            dto.GeometryJson,
            dto.Color
        );

        await _maps.AddZoneAsync(zone, ct);
        await _uow.SaveChangesAsync(ct);

        return Ok(new { id = zone.Id });
    }

    [HttpPut("zones/{zoneId:guid}")]
    [Authorize(Policy = "ManageDeviceConfigs")]
    public async Task<IActionResult> UpdateZone(Guid zoneId, [FromBody] UpdateFloorMapZoneDto dto, CancellationToken ct)
    {
        var zone = await _maps.GetZoneByIdAsync(zoneId, ct);

        if (zone is null)
            return NotFound(new { error = "Zone not found." });

        zone.Update(
            dto.Name,
            dto.ZoneType,
            dto.GeometryJson,
            dto.Color,
            dto.IsActive
        );

        await _maps.UpdateZoneAsync(zone, ct);
        await _uow.SaveChangesAsync(ct);

        return Ok(new { status = "updated" });
    }

    [HttpDelete("zones/{zoneId:guid}")]
    [Authorize(Policy = "ManageDeviceConfigs")]
    public async Task<IActionResult> DeleteZone(Guid zoneId, CancellationToken ct)
    {
        var zone = await _maps.GetZoneByIdAsync(zoneId, ct);

        if (zone is null)
            return NotFound(new { error = "Zone not found." });

        zone.SoftDelete(CurrentUserId);

        await _maps.UpdateZoneAsync(zone, ct);
        await _uow.SaveChangesAsync(ct);

        return NoContent();
    }

    private static FloorMapCalibrationDto ToCalibrationDto(FloorMapCalibration x)
    {
        return new FloorMapCalibrationDto(
            x.Id,
            x.FloorMapId,
            x.Name,
            x.SourceOriginX,
            x.SourceOriginY,
            x.SourceOriginZ,
            x.MapOriginX,
            x.MapOriginY,
            x.MapOriginZ,
            x.ScaleX,
            x.ScaleY,
            x.ScaleZ,
            x.RotationDegrees,
            x.IsDefault,
            x.IsActive
        );
    }

    private static FloorMapZoneDto ToZoneDto(FloorMapZone x)
    {
        return new FloorMapZoneDto(
            x.Id,
            x.FloorMapId,
            x.Name,
            x.ZoneType.ToString(),
            x.Color,
            x.GeometryJson,
            x.IsActive
        );
    }
}