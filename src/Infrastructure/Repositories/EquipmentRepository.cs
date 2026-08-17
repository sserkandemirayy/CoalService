using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class EquipmentRepository : IEquipmentRepository
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public EquipmentRepository(
        AppDbContext db,
        ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Equipment?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default)
    {
        return await ApplyScope(_db.Equipments)
            .Include(x => x.Category)
            .Include(x => x.Company)
            .Include(x => x.Branch)
            .Include(x => x.FloorMap)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                ct);
    }

    public async Task<Equipment?> GetByCodeAsync(
        Guid companyId,
        string code,
        CancellationToken ct = default)
    {
        return await ApplyScope(_db.Equipments)
            .FirstOrDefaultAsync(
                x =>
                    x.CompanyId == companyId &&
                    x.Code.ToLower() == code.ToLower(),
                ct);
    }

    public async Task<(IReadOnlyList<Equipment> Items, int Total)>
        GetPagedAsync(
            string? search,
            Guid? companyId,
            Guid? branchId,
            Guid? categoryId,
            Guid? floorMapId,
            EquipmentStatus? status,
            bool? isActive,
            int page,
            int pageSize,
            CancellationToken ct = default)
    {
        var query = ApplyScope(_db.Equipments)
            .Include(x => x.Category)
            .Include(x => x.Company)
            .Include(x => x.Branch)
            .Include(x => x.FloorMap)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            query = query.Where(x =>
                x.Code.Contains(search) ||
                x.Name.Contains(search) ||
                (x.SerialNumber != null &&
                 x.SerialNumber.Contains(search)) ||
                (x.Manufacturer != null &&
                 x.Manufacturer.Contains(search)) ||
                (x.Model != null &&
                 x.Model.Contains(search)));
        }

        if (companyId.HasValue)
        {
            query = query.Where(
                x => x.CompanyId == companyId.Value);
        }

        if (branchId.HasValue)
        {
            query = query.Where(
                x => x.BranchId == branchId.Value);
        }

        if (categoryId.HasValue)
        {
            query = query.Where(
                x => x.CategoryId == categoryId.Value);
        }

        if (floorMapId.HasValue)
        {
            query = query.Where(
                x => x.FloorMapId == floorMapId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(
                x => x.Status == status.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(
                x => x.IsActive == isActive.Value);
        }

        page = page <= 0 ? 1 : page;
        pageSize = pageSize <= 0 ? 20 : pageSize;
        pageSize = Math.Min(pageSize, 200);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Code)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<IReadOnlyList<Equipment>> GetMapItemsAsync(
        Guid floorMapId,
        CancellationToken ct = default)
    {
        return await ApplyScope(_db.Equipments)
            .Include(x => x.Category)
            .Where(x =>
                x.FloorMapId == floorMapId &&
                x.IsActive &&
                x.Category.IsActive &&
                x.Category.ShowOnMap &&
                x.X.HasValue &&
                x.Y.HasValue)
            .OrderBy(x => x.Category.Name)
            .ThenBy(x => x.Name)
            .ToListAsync(ct);
    }

    public async Task AddAsync(
        Equipment equipment,
        CancellationToken ct = default)
    {
        await _db.Equipments.AddAsync(
            equipment,
            ct);
    }

    public Task UpdateAsync(
        Equipment equipment,
        CancellationToken ct = default)
    {
        _db.Equipments.Update(equipment);
        return Task.CompletedTask;
    }

    public IQueryable<Equipment> Query()
    {
        return _db.Equipments.AsQueryable();
    }

    private IQueryable<Equipment> ApplyScope(
        IQueryable<Equipment> query)
    {
        if (HasUnrestrictedScope())
            return query;

        var companyIds =
            _currentUser.GetCurrentUserCompanyIds();

        var branchIds =
            _currentUser.GetCurrentUserBranchIds();

        return query.Where(x =>
            companyIds.Contains(x.CompanyId) ||
            (x.BranchId.HasValue &&
             branchIds.Contains(x.BranchId.Value)));
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