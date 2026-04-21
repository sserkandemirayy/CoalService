namespace Domain.Abstractions;

public interface ICurrentUserService
{
    Guid GetCurrentUserId();
    string? GetIpAddress();
    string? GetEmail();
    IEnumerable<string> GetRoles();
    IEnumerable<string> GetPermissions();
    bool IsSystemUser();
    List<Guid> GetCurrentUserCompanyIds();
}
