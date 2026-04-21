using Domain.Abstractions;

namespace Domain.Entities;

public class RolePermission : BaseEntity
{
    protected RolePermission() { }

    public Guid RoleId { get; private set; }
    public Role Role { get; private set; } = default!;

    public Guid PermissionId { get; private set; }
    public Permission Permission { get; private set; } = default!;

    private RolePermission(Guid roleId, Guid permissionId)
    {
        RoleId = roleId;
        PermissionId = permissionId;
    }

    public static RolePermission Create(Guid roleId, Guid permissionId)
        => new(roleId, permissionId);
}