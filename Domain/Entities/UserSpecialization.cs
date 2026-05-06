using Domain.Abstractions;

namespace Domain.Entities;

public class UserSpecialization : BaseEntity
{
    protected UserSpecialization() { }

    public Guid UserTypeId { get; private set; }
    public UserType UserType { get; private set; } = default!;

    public string Code { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }

    public bool IsSystem { get; private set; } = false;

    public ICollection<User> Users { get; private set; } = new List<User>();

    public static UserSpecialization Create(
        Guid userTypeId,
        string code,
        string name,
        string? description = null,
        bool isSystem = false)
        => new()
        {
            Id = Guid.NewGuid(),
            UserTypeId = userTypeId,
            Code = code,
            Name = name,
            Description = description,
            IsSystem = isSystem
        };

    public void Update(string name, string? description = null)
    {
        Name = name;
        Description = description;
    }
}