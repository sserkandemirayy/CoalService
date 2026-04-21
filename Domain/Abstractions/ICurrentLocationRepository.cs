using Domain.Entities;

namespace Domain.Abstractions;

public interface ICurrentLocationRepository
{
    Task<CurrentLocation?> GetByTagIdAsync(Guid tagId, CancellationToken ct = default);
    Task AddAsync(CurrentLocation currentLocation, CancellationToken ct = default);
    Task UpdateAsync(CurrentLocation currentLocation, CancellationToken ct = default);

    Task<IReadOnlyList<CurrentLocation>> GetFilteredAsync(
        Guid? userId,
        Guid? tagId,
        CancellationToken ct = default);

    IQueryable<CurrentLocation> Query();
}