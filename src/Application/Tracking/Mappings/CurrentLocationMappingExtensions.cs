using Application.Common.Mappings;
using Application.DTOs.Tracking;
using Domain.Entities;

namespace Application.Tracking.Mappings;

public static class CurrentLocationMappingExtensions
{
    public static CurrentLocationDto ToDto(
        this CurrentLocation currentLocation)
    {
        ArgumentNullException.ThrowIfNull(currentLocation);

        return new CurrentLocationDto(
            currentLocation.Id,
            currentLocation.TagId,
            currentLocation.Tag.ExternalId,
            currentLocation.Tag.Code,
            currentLocation.Tag.TagType.ToString(),
            currentLocation.UserId,
            currentLocation.User.GetFullName(),
            currentLocation.User?.Identifier,
            currentLocation.FloorMapId,
            currentLocation.FloorMapZoneId,
            currentLocation.X,
            currentLocation.Y,
            currentLocation.Z,
            currentLocation.Accuracy,
            currentLocation.Confidence,
            currentLocation.LastEventAt,
            currentLocation.LastRawEventId,
            currentLocation.LastKnownAnchorCount);
    }

    public static IReadOnlyList<CurrentLocationDto> ToDtoList(
        this IEnumerable<CurrentLocation> currentLocations)
    {
        ArgumentNullException.ThrowIfNull(currentLocations);

        return currentLocations
            .Select(x => x.ToDto())
            .ToList();
    }
}