using Domain.Abstractions;

namespace Domain.Entities;

public class FloorMapCalibration : BaseEntity
{
    protected FloorMapCalibration() { }

    public Guid FloorMapId { get; private set; }
    public FloorMap FloorMap { get; private set; } = default!;

    public string Name { get; private set; } = default!;

    public decimal SourceOriginX { get; private set; }
    public decimal SourceOriginY { get; private set; }
    public decimal SourceOriginZ { get; private set; }

    public decimal MapOriginX { get; private set; }
    public decimal MapOriginY { get; private set; }
    public decimal MapOriginZ { get; private set; }

    public decimal ScaleX { get; private set; }
    public decimal ScaleY { get; private set; }
    public decimal ScaleZ { get; private set; }

    public decimal RotationDegrees { get; private set; }

    public bool IsDefault { get; private set; }
    public bool IsActive { get; private set; }

    public static FloorMapCalibration CreateIdentity(Guid floorMapId, string name = "Default Identity Calibration")
    {
        return new FloorMapCalibration
        {
            FloorMapId = floorMapId,
            Name = name,
            SourceOriginX = 0,
            SourceOriginY = 0,
            SourceOriginZ = 0,
            MapOriginX = 0,
            MapOriginY = 0,
            MapOriginZ = 0,
            ScaleX = 1,
            ScaleY = 1,
            ScaleZ = 1,
            RotationDegrees = 0,
            IsDefault = true,
            IsActive = true
        };
    }

    public static FloorMapCalibration Create(
        Guid floorMapId,
        string name,
        decimal sourceOriginX,
        decimal sourceOriginY,
        decimal sourceOriginZ,
        decimal mapOriginX,
        decimal mapOriginY,
        decimal mapOriginZ,
        decimal scaleX,
        decimal scaleY,
        decimal scaleZ,
        decimal rotationDegrees,
        bool isDefault)
    {
        return new FloorMapCalibration
        {
            FloorMapId = floorMapId,
            Name = string.IsNullOrWhiteSpace(name) ? "Calibration" : name.Trim(),
            SourceOriginX = sourceOriginX,
            SourceOriginY = sourceOriginY,
            SourceOriginZ = sourceOriginZ,
            MapOriginX = mapOriginX,
            MapOriginY = mapOriginY,
            MapOriginZ = mapOriginZ,
            ScaleX = scaleX == 0 ? 1 : scaleX,
            ScaleY = scaleY == 0 ? 1 : scaleY,
            ScaleZ = scaleZ == 0 ? 1 : scaleZ,
            RotationDegrees = rotationDegrees,
            IsDefault = isDefault,
            IsActive = true
        };
    }

    public void Update(
        string name,
        decimal sourceOriginX,
        decimal sourceOriginY,
        decimal sourceOriginZ,
        decimal mapOriginX,
        decimal mapOriginY,
        decimal mapOriginZ,
        decimal scaleX,
        decimal scaleY,
        decimal scaleZ,
        decimal rotationDegrees,
        bool isDefault,
        bool isActive)
    {
        Name = string.IsNullOrWhiteSpace(name) ? "Calibration" : name.Trim();
        SourceOriginX = sourceOriginX;
        SourceOriginY = sourceOriginY;
        SourceOriginZ = sourceOriginZ;
        MapOriginX = mapOriginX;
        MapOriginY = mapOriginY;
        MapOriginZ = mapOriginZ;
        ScaleX = scaleX == 0 ? 1 : scaleX;
        ScaleY = scaleY == 0 ? 1 : scaleY;
        ScaleZ = scaleZ == 0 ? 1 : scaleZ;
        RotationDegrees = rotationDegrees;
        IsDefault = isDefault;
        IsActive = isActive;
    }
}