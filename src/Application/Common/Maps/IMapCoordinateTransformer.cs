using Domain.Entities;

namespace Application.Common.Maps;

public interface IMapCoordinateTransformer
{
    MapCoordinate TransformToMap(FloorMapCalibration calibration, decimal sourceX, decimal sourceY, decimal sourceZ);
}

public sealed record MapCoordinate(decimal X, decimal Y, decimal Z);