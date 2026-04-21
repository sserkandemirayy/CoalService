using Domain.Entities;

namespace Domain.Abstractions;

public interface IBranchRepository
{
    Task<Branch?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Branch branch, CancellationToken ct = default);
    Task UpdateAsync(Branch branch, CancellationToken ct = default);
    Task<IEnumerable<Branch>> GetByCompanyIdAsync(Guid companyId, CancellationToken ct = default);
}
