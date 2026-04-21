using Application.DTOs.Permissions;
using Domain.Entities;
public record RoleDto(Guid Id, string Name, string? Description, bool BuiltIn, int UsersCount, List<PermissionDto> Permissions)
{
    public static RoleDto FromEntity(Role r)
        => new(
            r.Id,           
            r.Name,
            r.Description,
            r.Name is "admin" or "user" or "patient" or "doctor" or "staff", // built-in örneği
            r.UserRoles?.Count ?? 0,
            r.RolePermissions?.Select(rp => PermissionDto.FromEntity(rp.Permission)).ToList() ?? new()
        );
}
