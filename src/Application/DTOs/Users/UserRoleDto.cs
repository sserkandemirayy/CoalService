using Domain.Entities;

namespace Application.DTOs.Users;

public record UserRoleDto(Guid RoleId, string RoleName)
{
    public static UserRoleDto FromEntity(UserRole ur)
        => new(ur.RoleId, ur.Role.Name);
}
