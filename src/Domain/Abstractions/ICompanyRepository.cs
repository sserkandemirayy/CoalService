using Domain.Entities;

namespace Domain.Abstractions;

public interface ICompanyRepository
{
    Task<Company?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Company company, CancellationToken ct = default);
    Task UpdateAsync(Company company, CancellationToken ct = default);
    Task<IEnumerable<Company>> GetAllAsync(CancellationToken ct = default);
}
