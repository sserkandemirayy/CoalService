using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CurrentLocationRepository : ICurrentLocationRepository
{
    private readonly AppDbContext _db;

    public CurrentLocationRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<CurrentLocation?> GetByTagIdAsync(Guid tagId, CancellationToken ct = default)
        => await _db.CurrentLocations.FirstOrDefaultAsync(x => x.TagId == tagId, ct);

    public async Task AddAsync(CurrentLocation currentLocation, CancellationToken ct = default)
        => await _db.CurrentLocations.AddAsync(currentLocation, ct);

    public Task UpdateAsync(CurrentLocation currentLocation, CancellationToken ct = default)
    {
        _db.CurrentLocations.Update(currentLocation);
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<CurrentLocation>> GetFilteredAsync(
        Guid? userId,
        Guid? tagId,
        CancellationToken ct = default)
    {
        var query = _db.CurrentLocations.AsQueryable();

        if (userId.HasValue)
            query = query.Where(x => x.UserId == userId.Value);

        if (tagId.HasValue)
            query = query.Where(x => x.TagId == tagId.Value);

        return await query
            .OrderByDescending(x => x.LastEventAt)
            .ToListAsync(ct);
    }

    public IQueryable<CurrentLocation> Query() => _db.CurrentLocations.AsQueryable();
}