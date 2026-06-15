using Application.Common.Maps;
using Domain.Entities;

namespace Infrastructure.Services;

public class MapCoordinateTransformer : IMapCoordinateTransformer
{
    public MapCoordinate TransformToMap(
        FloorMapCalibration calibration,
        decimal sourceX,
        decimal sourceY,
        decimal sourceZ)
    {
        var dx = (sourceX - calibration.SourceOriginX) * calibration.ScaleX;
        var dy = (sourceY - calibration.SourceOriginY) * calibration.ScaleY;
        var dz = (sourceZ - calibration.SourceOriginZ) * calibration.ScaleZ;

        var angle = (double)calibration.RotationDegrees * Math.PI / 180d;

        var cos = (decimal)Math.Cos(angle);
        var sin = (decimal)Math.Sin(angle);

        var rotatedX = dx * cos - dy * sin;
        var rotatedY = dx * sin + dy * cos;

        return new MapCoordinate(
            calibration.MapOriginX + rotatedX,
            calibration.MapOriginY + rotatedY,
            calibration.MapOriginZ + dz
        );
    }
}