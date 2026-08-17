using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class EquipmentCategoryRepository
    : IEquipmentCategoryRepository
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public EquipmentCategoryRepository(
        AppDbContext db,
        ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<EquipmentCategory?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default)
    {
        return await ApplyScope(_db.EquipmentCategories)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<EquipmentCategory?> GetByCodeAsync(
        Guid companyId,
        string code,
        CancellationToken ct = default)
    {
        return await ApplyScope(_db.EquipmentCategories)
            .FirstOrDefaultAsync(
                x =>
                    x.CompanyId == companyId &&
                    x.Code.ToLower() == code.ToLower(),
                ct);
    }

    public async Task<IReadOnlyList<EquipmentCategory>> GetAllAsync(
        Guid? companyId = null,
        bool? isActive = null,
        CancellationToken ct = default)
    {
        var query = ApplyScope(_db.EquipmentCategories);

        if (companyId.HasValue)
        {
            query = query.Where(
                x => x.CompanyId == companyId.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(
                x => x.IsActive == isActive.Value);
        }

        return await query
            .OrderBy(x => x.Name)
            .ToListAsync(ct);
    }

    public async Task AddAsync(
        EquipmentCategory category,
        CancellationToken ct = default)
    {
        await _db.EquipmentCategories.AddAsync(
            category,
            ct);
    }

    public Task UpdateAsync(
        EquipmentCategory category,
        CancellationToken ct = default)
    {
        _db.EquipmentCategories.Update(category);
        return Task.CompletedTask;
    }

    public IQueryable<EquipmentCategory> Query()
    {
        return _db.EquipmentCategories.AsQueryable();
    }

    private IQueryable<EquipmentCategory> ApplyScope(
        IQueryable<EquipmentCategory> query)
    {
        if (HasUnrestrictedScope())
            return query;

        var companyIds =
            _currentUser.GetCurrentUserCompanyIds();

        return query.Where(
            x => companyIds.Contains(x.CompanyId));
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