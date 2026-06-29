using Application.Common.Notifications;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public sealed class NotificationTargetResolver : INotificationTargetResolver
{
    private readonly AppDbContext _db;

    public NotificationTargetResolver(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Guid>> ResolveAsync(
        NotificationTargetType targetType,
        IReadOnlyList<Guid>? targetIds,
        CancellationToken ct = default)
    {
        var ids = targetIds?.Where(x => x != Guid.Empty).Distinct().ToList() ?? new List<Guid>();

        return targetType switch
        {
            NotificationTargetType.Broadcast => await _db.Users
                .Where(x => x.IsActive)
                .Select(x => x.Id)
                .Distinct()
                .ToListAsync(ct),

            NotificationTargetType.Users => await _db.Users
                .Where(x => x.IsActive && ids.Contains(x.Id))
                .Select(x => x.Id)
                .Distinct()
                .ToListAsync(ct),

            NotificationTargetType.Roles => await _db.UserRoles
                .Where(x => ids.Contains(x.RoleId) && x.User.IsActive)
                .Select(x => x.UserId)
                .Distinct()
                .ToListAsync(ct),

            NotificationTargetType.Company => await _db.UserCompanies
                .Where(x => ids.Contains(x.CompanyId) && x.User.IsActive)
                .Select(x => x.UserId)
                .Distinct()
                .ToListAsync(ct),

            NotificationTargetType.Branch => await _db.UserBranches
                .Where(x => ids.Contains(x.BranchId) && x.User.IsActive)
                .Select(x => x.UserId)
                .Distinct()
                .ToListAsync(ct),

            _ => new List<Guid>()
        };
    }

    public async Task<IReadOnlyList<Guid>> ResolveByPermissionAsync(
        string permissionName,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(permissionName))
            return new List<Guid>();

        var permission = permissionName.Trim().ToLower();

        return await _db.UserRoles
            .Where(ur =>
                ur.User.IsActive &&
                ur.Role.RolePermissions.Any(rp => rp.Permission.Name.ToLower() == permission))
            .Select(ur => ur.UserId)
            .Distinct()
            .ToListAsync(ct);
    }
}