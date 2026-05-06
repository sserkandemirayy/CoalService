using Domain.Abstractions;

namespace Domain.Entities;

public class Permission : BaseEntity
{
    private Permission() { }

    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }

    public ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();

    public static Permission Create(string name, string? description = null)
        => new Permission { Name = name, Description = description };

    public void Update(string name, string? description)
    {
        Name = name;
        Description = description;
    }
}
