using Domain.Entities;

namespace Domain.Abstractions;

public interface IUserBranchRepository
{
    Task AddOrReactivateAsync(Guid userId, Guid branchId, Guid performedBy, CancellationToken ct = default);
    Task RemoveAsync(Guid userId, Guid branchId, Guid performedBy, CancellationToken ct = default);

    Task<IEnumerable<Guid>> GetUserBranchIdsAsync(Guid userId, CancellationToken ct = default);
    Task<IEnumerable<Branch>> GetBranchesByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<IEnumerable<User>> GetUsersByBranchIdAsync(Guid branchId, CancellationToken ct = default);
}