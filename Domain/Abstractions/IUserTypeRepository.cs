using Domain.Entities;

namespace Domain.Abstractions;

public interface IUserTypeRepository
{
    Task<IEnumerable<UserType>> GetAllAsync(CancellationToken ct = default);
    Task<UserType?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<UserType?> FindByCodeAsync(string name, CancellationToken cancellationToken = default);
  
}