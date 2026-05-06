using Domain.Entities;

namespace Domain.Abstractions;

public interface IUserRepository
{
    Task<User?> FindByEmailAsync(string email, CancellationToken ct = default);
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(User user, CancellationToken ct = default);

    Task<IEnumerable<string>> GetRoleNamesAsync(Guid userId, CancellationToken ct = default);
    Task<IEnumerable<string>> GetPermissionsAsync(Guid userId, CancellationToken ct = default);

    Task AssignRoleAsync(User user, Role role, CancellationToken ct = default);

    Task UpdateAsync(User user, CancellationToken ct = default);
    Task RecordLoginSuccessAsync(User user, CancellationToken ct = default);
    Task RecordLoginFailureAsync(User user, int maxAttempts, TimeSpan lockoutDuration, CancellationToken ct = default);

    Task<IEnumerable<User>> GetAllAsync(CancellationToken ct = default);

    Task<List<Guid>> GetUserIdsByRoleAsync(string roleName, CancellationToken ct = default);

    Task<IEnumerable<string>> GetPermissionNamesAsync(Guid userId, CancellationToken ct);
    Task<bool> UserHasPermissionAsync(Guid userId, string permissionName, CancellationToken ct);

    IQueryable<User> Query();

    //Task<(List<User> Users, int Total)> GetPagedFilteredAsync(
    //    string? q,
    //    string? role,
    //    string? status,
    //    string? sort,
    //    int page,
    //    int pageSize,
    //    CancellationToken ct);

    Task<(IEnumerable<User> Users, int Total)> GetFilteredPagedAsync(
                string? q,
                string? role,
                string? status,
                Guid? specializationId,
                List<string>? userTypeCodes,
                int page,
                int pageSize,
                string? sort,
                CancellationToken ct);

    Task<User?> GetByIdForUpdateAsync(Guid id, CancellationToken ct = default);

    Task<IEnumerable<User>> GetPatientsAsync(CancellationToken ct);
    Task<User?> GetPatientByIdAsync(Guid id, CancellationToken ct = default);
}

