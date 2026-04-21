using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly AppDbContext _dbContext;

    public RoleRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .Include(r => r.UserRoles)
                .ThenInclude(ur => ur.User)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<Role?> FindByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Name.ToLower() == name.ToLower(), cancellationToken);
    }


    public async Task<IEnumerable<Role>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Role role, CancellationToken cancellationToken = default)
    {
        await _dbContext.Roles.AddAsync(role, cancellationToken);
    }

    public void Remove(Role role)
    {
        _dbContext.Roles.Remove(role);
    }

    // Reactivation destekli permission ekleme (Role.AssignPermission yerine kullanilabilir)
    public async Task AssignPermissionAsync(Role role, Permission permission, CancellationToken ct = default)
    {
        var existing = await _dbContext.RolePermissions
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.RoleId == role.Id && x.PermissionId == permission.Id, ct);

        if (existing != null)
        {
            if (existing.DeletedAt != null)
            {
                existing.DeletedAt = null;
                existing.DeletedBy = null;
                existing.UpdatedAt = DateTime.UtcNow;
            }
        }
        else
        {
            await _dbContext.RolePermissions.AddAsync(RolePermission.Create(role.Id, permission.Id), ct);
        }
    }
}