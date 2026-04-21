using Domain.Abstractions;

namespace Domain.Entities;

public class Role : BaseEntity
{
    private Role() { }


    public string Name { get; private set; } = default!;


    public string? Description { get; private set; }

    public ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();
    public ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();

    // ==== Factory ====
    public static Role Create(string name, string? description = null)
        => new Role { Name = name, Description = description};

    // ==== Güncelleme ====
    public void Update(string name, string? description)
    {
        Name = name;
        Description = description;
    }

    // ==== Permission Ýþlemleri ====
    public void AssignPermission(Permission permission)
    {
        var existing = RolePermissions.FirstOrDefault(rp => rp.PermissionId == permission.Id);

        if (existing != null)
        {
            if (existing.DeletedAt != null)
            {
                existing.DeletedAt = null;
                existing.DeletedBy = null;
            }
            return;
        }

        RolePermissions.Add(RolePermission.Create(Id, permission.Id));
    }

    public void RemovePermission(Permission permission)
    {
        var rp = RolePermissions.FirstOrDefault(r => r.PermissionId == permission.Id && r.DeletedAt == null);
        if (rp != null)
        {
            rp.DeletedAt = DateTime.UtcNow;
            rp.DeletedBy = Id;
        }
    }
}
