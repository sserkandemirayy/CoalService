public class SyncRolesDto
{
    public List<Guid> Users { get; set; } = new();
    public List<Guid> Add { get; set; } = new();
    public List<Guid> Remove { get; set; } = new();
}
