using Domain.Entities;
using Domain.Abstractions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;
    public UserRepository(AppDbContext db) => _db = db;

    public async Task<User?> FindByEmailAsync(string email, CancellationToken ct = default)
        => await _db.Users.FirstOrDefaultAsync(x => x.Email == email, ct);

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
    => await _db.Users
        .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
        .Include(u => u.UserType)
        .Include(u => u.Specialization)
        .Include(u => u.UserCompanies)
            .ThenInclude(uc => uc.Company)
        .Include(u => u.UserBranches)
            .ThenInclude(ub => ub.Branch).ThenInclude(b => b.Company)
        .AsNoTracking()
        .FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task AddAsync(User user, CancellationToken ct = default)
        => await _db.Users.AddAsync(user, ct);

    public async Task<IEnumerable<string>> GetRoleNamesAsync(Guid userId, CancellationToken ct = default)
        => await _db.UserRoles
            .Where(ur => ur.UserId == userId)
            .Include(ur => ur.Role)
            .Select(ur => ur.Role.Name)
            .ToListAsync(ct);

    public async Task<IEnumerable<string>> GetPermissionsAsync(Guid userId, CancellationToken ct = default)
        => await _db.UserRoles
            .Where(ur => ur.UserId == userId)
            .Include(ur => ur.Role).ThenInclude(r => r.RolePermissions).ThenInclude(rp => rp.Permission)
            .SelectMany(ur => ur.Role.RolePermissions.Select(rp => rp.Permission.Name))
            .Distinct()
            .ToListAsync(ct);

    public async Task AssignRoleAsync(User user, Role role, CancellationToken ct = default)
    {
        var existing = await _db.UserRoles
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.UserId == user.Id && x.RoleId == role.Id, ct);

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
            await _db.UserRoles.AddAsync(UserRole.Create(user.Id, role.Id), ct);
        }
    }

    public async Task UpdateAsync(User user, CancellationToken ct = default)
    {
        _db.Users.Update(user);
        await _db.SaveChangesAsync(ct);
    }

    public async Task RecordLoginSuccessAsync(User user, CancellationToken ct = default)
    {
        user.RecordLoginSuccess();
        _db.Users.Update(user);
        await _db.SaveChangesAsync(ct);
    }

    public async Task RecordLoginFailureAsync(User user, int maxAttempts, TimeSpan lockoutDuration, CancellationToken ct = default)
    {
        user.RecordLoginFailure(maxAttempts, lockoutDuration);
        _db.Users.Update(user);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<User>> GetAllAsync(CancellationToken ct = default)
        => await _db.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .Include(u => u.UserType)
            .Include(u => u.Specialization)
            .Include(u => u.UserCompanies).ThenInclude(uc => uc.Company)
            .Include(u => u.UserBranches)             
            .ThenInclude(ub => ub.Branch)
            .ToListAsync(ct);

    public async Task<List<Guid>> GetUserIdsByRoleAsync(string roleName, CancellationToken ct = default)
        => await _db.UserRoles
            .Include(ur => ur.Role)
            .Where(ur => ur.Role.Name.ToLower() == roleName.ToLower())
            .Select(ur => ur.UserId)
            .Distinct()
            .ToListAsync(ct);

    public async Task<IEnumerable<string>> GetPermissionNamesAsync(Guid userId, CancellationToken ct)
        => await _db.Users
            .Where(u => u.Id == userId)
            .SelectMany(u => u.UserRoles)
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Name)
            .Distinct()
            .ToListAsync(ct);

    public async Task<bool> UserHasPermissionAsync(Guid userId, string permissionName, CancellationToken ct)
        => await _db.Users
            .Where(u => u.Id == userId)
            .AnyAsync(u => u.UserRoles.Any(ur => ur.Role.RolePermissions.Any(rp => rp.Permission.Name == permissionName)), ct);

    public IQueryable<User> Query() => _db.Users.AsQueryable();
    public async Task<(IEnumerable<User> Users, int Total)> GetFilteredPagedAsync(
    string? q,
    string? role,
    string? status,
    Guid? specializationId,
    List<string>? userTypeCodes,
    int page,
    int pageSize,
    string? sort,
    CancellationToken ct)
    {
        var query = _db.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .Include(u => u.UserType)
            .Include(u => u.Specialization)
            .Include(u => u.UserCompanies).ThenInclude(uc => uc.Company)
            .AsNoTracking();

        if (userTypeCodes != null)
        {
            userTypeCodes = userTypeCodes
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            if (userTypeCodes.Count == 0)
                userTypeCodes = null;
        }

        if (specializationId.HasValue && specializationId.Value == Guid.Empty)
            specializationId = null;

        // Search
        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(u =>
                u.FirstName.Contains(q) ||
                u.LastName.Contains(q) ||
                u.Email.Contains(q));

        // Role
        if (!string.IsNullOrWhiteSpace(role))
            query = query.Where(u => u.UserRoles.Any(ur => ur.Role.Name == role));

        // Status
        if (!string.IsNullOrWhiteSpace(status))
        {
            query = status.ToLower() switch
            {
                "active" => query.Where(u => u.IsActive),
                "inactive" => query.Where(u => !u.IsActive),
                _ => query
            };
        }

        // === NEW: UserType code filter ===
        if (userTypeCodes is { Count: > 0 })
        {
            var codes = userTypeCodes.Select(x => x.ToLower()).ToList();
            query = query.Where(u => u.UserType != null &&
                                     codes.Contains(u.UserType.Code.ToLower()));
        }

        // === NEW: Specialization filter ===
        if (specializationId.HasValue)
        {
            query = query.Where(u => u.SpecializationId == specializationId.Value);
        }

        // Total BEFORE paging
        var total = await query.CountAsync(ct);

        // Sort
        query = sort?.ToLower() switch
        {
            "lastname" => query.OrderBy(u => u.LastName),
            "email" => query.OrderBy(u => u.Email),
            "createdat" => query.OrderByDescending(u => u.CreatedAt),
            _ => query.OrderBy(u => u.FirstName)
        };

        var users = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (users, total);
    }

    public async Task<User?> GetByIdForUpdateAsync(Guid id, CancellationToken ct = default)
    {
        return await _db.Users
            .Include(u => u.Specialization)
            .Include(u => u.UserType)
            .Include(u => u.UserCompanies)
            .Include(u => u.UserBranches)
            .FirstOrDefaultAsync(u => u.Id == id, ct); 
    }

    public async Task<IEnumerable<User>> GetPatientsAsync(CancellationToken ct = default)
        => await _db.Users
            .Include(u => u.UserType)
            .Include(u => u.Specialization) 
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .Where(u =>
                u.DeletedAt == null &&
                u.UserType != null &&
                u.UserType.Code != null &&
                u.UserType.Code.ToLower() == "patient")
            .OrderByDescending(u => u.CreatedAt)
            .AsNoTracking()
            .ToListAsync(ct);

    public async Task<User?> GetPatientByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.Users
            .Include(u => u.UserType)
            .Include(u => u.Specialization) 
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u =>
                u.Id == id &&
                u.DeletedAt == null &&
                u.UserType != null &&
                u.UserType.Code != null &&
                u.UserType.Code.ToLower() == "patient", ct);


}