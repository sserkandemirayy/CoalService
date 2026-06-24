using System.Text.Json;
using Application.Common.Maps;
using Domain.Entities;

namespace Infrastructure.Services;

public sealed class MapZoneResolver : IMapZoneResolver
{
    public Guid? ResolveZoneId(IEnumerable<FloorMapZone> zones, decimal x, decimal y)
    {
        foreach (var zone in zones.Where(z => z.IsActive))
        {
            if (IsInside(zone.GeometryJson, x, y))
                return zone.Id;
        }

        return null;
    }

    private static bool IsInside(string geometryJson, decimal x, decimal y)
    {
        if (string.IsNullOrWhiteSpace(geometryJson))
            return false;

        List<PointDto>? points;

        try
        {
            points = JsonSerializer.Deserialize<List<PointDto>>(
                geometryJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch
        {
            return false;
        }

        if (points is null || points.Count < 3)
            return false;

        var inside = false;
        var j = points.Count - 1;

        for (var i = 0; i < points.Count; i++)
        {
            var xi = points[i].X;
            var yi = points[i].Y;
            var xj = points[j].X;
            var yj = points[j].Y;

            var intersect =
                ((yi > y) != (yj > y)) &&
                (x < (xj - xi) * (y - yi) / ((yj - yi) == 0 ? 0.000001m : (yj - yi)) + xi);

            if (intersect)
                inside = !inside;

            j = i;
        }

        return inside;
    }

    private sealed record PointDto(decimal X, decimal Y);
}