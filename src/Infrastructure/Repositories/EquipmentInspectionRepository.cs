using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class EquipmentInspectionRepository
    : IEquipmentInspectionRepository
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public EquipmentInspectionRepository(
        AppDbContext db,
        ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<EquipmentInspection?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default)
    {
        return await ApplyScope(_db.EquipmentInspections)
            .Include(x => x.InspectedByUser)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                ct);
    }

    public async Task<IReadOnlyList<EquipmentInspection>>
        GetByEquipmentIdAsync(
            Guid equipmentId,
            CancellationToken ct = default)
    {
        return await ApplyScope(_db.EquipmentInspections)
            .Include(x => x.InspectedByUser)
            .Where(x => x.EquipmentId == equipmentId)
            .OrderByDescending(x => x.InspectedAt)
            .ToListAsync(ct);
    }

    public async Task AddAsync(
        EquipmentInspection inspection,
        CancellationToken ct = default)
    {
        await _db.EquipmentInspections.AddAsync(
            inspection,
            ct);
    }

    public IQueryable<EquipmentInspection> Query()
    {
        return _db.EquipmentInspections.AsQueryable();
    }

    private IQueryable<EquipmentInspection> ApplyScope(
        IQueryable<EquipmentInspection> query)
    {
        if (HasUnrestrictedScope())
            return query;

        var companyIds =
            _currentUser.GetCurrentUserCompanyIds();

        var branchIds =
            _currentUser.GetCurrentUserBranchIds();

        return query.Where(x =>
            companyIds.Contains(
                x.Equipment.CompanyId) ||
            (x.Equipment.BranchId.HasValue &&
             branchIds.Contains(
                 x.Equipment.BranchId.Value)));
    }

    private bool HasUnrestrictedScope()
    {
        return _currentUser.IsSystemUser() ||
               _currentUser.GetRoles().Any(
                   x => x.Equals(
                       "super_admin",
                       StringComparison.OrdinalIgnoreCase));
    }
}