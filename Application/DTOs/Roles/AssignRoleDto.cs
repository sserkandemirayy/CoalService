namespace Application.DTOs.Roles
{
    public class AssignRoleDto
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
    }

    public class AssignRolesDto
    {
        public List<Guid> Users { get; set; } = new();
        public List<Guid> Roles { get; set; } = new();
    }

    public class SyncRolePermissionsDto
    {
        public Guid RoleId { get; set; }
        public List<Guid> Add { get; set; } = new();
        public List<Guid> Remove { get; set; } = new();
    }

}