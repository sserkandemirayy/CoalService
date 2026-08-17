using Domain.Entities;

public interface IUserCompanyRepository
{
    Task<string?> AddOrReactivateAsync(
        Guid userId,
        Guid companyId,
        string? requestedUserCode = null,
        CancellationToken ct = default);

    Task RemoveAsync(
        Guid userId,
        Guid companyId,
        Guid performedBy,
        CancellationToken ct = default);

    Task<IEnumerable<Company>> GetCompaniesByUserIdAsync(
        Guid userId,
        CancellationToken ct = default);

    Task<IEnumerable<User>> GetUsersByCompanyIdAsync(
        Guid companyId,
        CancellationToken ct = default);

    Task<bool> IsUserInCompanyAsync(
        Guid userId,
        Guid companyId,
        CancellationToken ct = default);

    Task<bool> UserCodeExistsAsync(
        Guid companyId,
        string userCode,
        Guid? excludeUserId = null,
        CancellationToken ct = default);

    Task<bool> SetUserCodeAsync(
        Guid userId,
        Guid companyId,
        string userCode,
        CancellationToken ct = default);
}