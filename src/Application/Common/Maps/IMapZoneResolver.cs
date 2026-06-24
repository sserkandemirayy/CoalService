using Domain.Entities;

namespace Application.Common.Maps;

public interface IMapZoneResolver
{
    Guid? ResolveZoneId(IEnumerable<FloorMapZone> zones, decimal x, decimal y);
}