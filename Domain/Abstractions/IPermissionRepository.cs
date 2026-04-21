using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Abstractions
{
    public interface IPermissionRepository
    {
        Task<Permission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Permission?> FindByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<IEnumerable<Permission>> GetAllAsync(CancellationToken cancellationToken = default);
        Task AddAsync(Permission permission, CancellationToken cancellationToken = default);
        void Remove(Permission permission);
    }
}
