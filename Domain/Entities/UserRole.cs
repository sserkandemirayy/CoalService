using Domain.Abstractions;

namespace Domain.Entities;

public class UserRole : BaseEntity
{
    protected UserRole() { }

    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!;

    public Guid RoleId { get; private set; }
    public Role Role { get; private set; } = default!;

    private UserRole(Guid userId, Guid roleId)
    {
        UserId = userId;
        RoleId = roleId;
    }

    public static UserRole Create(Guid userId, Guid roleId)
        => new(userId, roleId);
}