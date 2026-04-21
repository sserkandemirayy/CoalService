using Domain.Entities;

namespace Domain.Services;

public static class UserDomainService
{
    public static void AssignRole(User user, Role role)
    {
        if (user.UserRoles.Any(ur => ur.RoleId == role.Id)) return;

        user.UserRoles.Add(UserRole.Create(user.Id, role.Id));
    }
}
