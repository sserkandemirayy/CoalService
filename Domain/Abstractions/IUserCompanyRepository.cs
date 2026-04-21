using Domain.Entities;

public interface IUserCompanyRepository
{
    Task AddOrReactivateAsync(Guid userId, Guid companyId, CancellationToken ct = default);

    Task RemoveAsync(Guid userId, Guid companyId, Guid performedBy, CancellationToken ct = default);

    Task<IEnumerable<Company>> GetCompaniesByUserIdAsync(Guid userId, CancellationToken ct = default);

    Task<IEnumerable<User>> GetUsersByCompanyIdAsync(Guid companyId, CancellationToken ct = default);

    Task<bool> IsUserInCompanyAsync(Guid userId, Guid companyId, CancellationToken ct = default);
}