using Domain.Abstractions;

namespace Domain.Entities;

public class UserType : BaseEntity
{
    protected UserType() { }

    public string Code { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }

    public bool IsSystem { get; private set; } = false;

    public ICollection<User> Users { get; private set; } = new List<User>();

    public ICollection<UserSpecialization> Specializations { get; private set; }
    = new List<UserSpecialization>();

    public static UserType Create(string code, string name, string? desc = null, bool isSystem = false)
        => new() { Code = code, Name = name, Description = desc, IsSystem= isSystem };

    public void Update(string name, string? desc = null)
    {
        Name = name;
        Description = desc;
    }
}