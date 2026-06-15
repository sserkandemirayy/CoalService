using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities;

public class FloorMapZone : BaseEntity
{
    protected FloorMapZone() { }

    public Guid FloorMapId { get; private set; }
    public FloorMap FloorMap { get; private set; } = default!;

    public string Name { get; private set; } = default!;
    public FloorMapZoneType ZoneType { get; private set; }
    public string? Color { get; private set; }

    public string GeometryJson { get; private set; } = default!;
    public bool IsActive { get; private set; } = true;

    public static FloorMapZone Create(
        Guid floorMapId,
        string name,
        FloorMapZoneType zoneType,
        string geometryJson,
        string? color = null)
    {
        return new FloorMapZone
        {
            FloorMapId = floorMapId,
            Name = name.Trim(),
            ZoneType = zoneType,
            GeometryJson = geometryJson,
            Color = color?.Trim(),
            IsActive = true
        };
    }

    public void Update(string name, FloorMapZoneType zoneType, string geometryJson, string? color, bool isActive)
    {
        Name = name.Trim();
        ZoneType = zoneType;
        GeometryJson = geometryJson;
        Color = color?.Trim();
        IsActive = isActive;
    }
}