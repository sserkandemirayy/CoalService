using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CurrentLocationRepository : ICurrentLocationRepository
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CurrentLocationRepository(AppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<CurrentLocation?> GetByTagIdAsync(Guid tagId, CancellationToken ct = default)
        => await ApplyScope(_db.CurrentLocations).FirstOrDefaultAsync(x => x.TagId == tagId, ct);

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
        var query = ApplyScope(_db.CurrentLocations);

        if (userId.HasValue)
            query = query.Where(x => x.UserId == userId.Value);

        if (tagId.HasValue)
            query = query.Where(x => x.TagId == tagId.Value);

        return await query
            .OrderByDescending(x => x.LastEventAt)
            .ToListAsync(ct);
    }

    public IQueryable<CurrentLocation> Query() => _db.CurrentLocations.AsQueryable();

    private IQueryable<CurrentLocation> ApplyScope(IQueryable<CurrentLocation> query)
    {
        if (HasUnrestrictedScope())
            return query;

        var companyIds = _currentUser.GetCurrentUserCompanyIds();
        var branchIds = _currentUser.GetCurrentUserBranchIds();

        return query.Where(x =>
            (x.UserId.HasValue && (
                x.User!.UserCompanies.Any(uc => companyIds.Contains(uc.CompanyId)) ||
                x.User.UserBranches.Any(ub => branchIds.Contains(ub.BranchId)))) ||
            x.Tag.Assignments.Any(a =>
                a.UnassignedAt == null &&
                (a.User.UserCompanies.Any(uc => companyIds.Contains(uc.CompanyId)) ||
                 a.User.UserBranches.Any(ub => branchIds.Contains(ub.BranchId)))));
    }

    private bool HasUnrestrictedScope()
        => _currentUser.IsSystemUser() ||
           _currentUser.GetRoles().Any(x => x.Equals("super_admin", StringComparison.OrdinalIgnoreCase));
}
