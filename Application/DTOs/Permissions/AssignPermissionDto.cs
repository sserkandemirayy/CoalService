namespace Application.DTOs.Permissions
{
    public class AssignPermissionDto
    {
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }
    }
}
