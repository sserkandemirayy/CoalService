using Domain.Entities;

namespace Domain.Abstractions
{
    public interface IRoleRepository
    {
        Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Role?> FindByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<IEnumerable<Role>> GetAllAsync(CancellationToken cancellationToken = default);
        Task AddAsync(Role role, CancellationToken cancellationToken = default);
        void Remove(Role role);
    }
}
