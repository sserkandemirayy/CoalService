namespace Application.DTOs.Permissions;

public record PermissionDto(Guid Id, string Name, string? Description)
{
    public static PermissionDto FromEntity(Domain.Entities.Permission p)
        => new(p.Id, p.Name, p.Description);
}
