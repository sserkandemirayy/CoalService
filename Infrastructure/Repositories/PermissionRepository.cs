using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly AppDbContext _dbContext;

        public PermissionRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Permission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Permissions
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<Permission?> FindByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Permissions
                .FirstOrDefaultAsync(p => p.Name.ToLower() == name.ToLower(), cancellationToken);
        }

        public async Task<IEnumerable<Permission>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Permissions.ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Permission permission, CancellationToken cancellationToken = default)
        {
            await _dbContext.Permissions.AddAsync(permission, cancellationToken);
        }

        public void Remove(Permission permission)
        {
            _dbContext.Permissions.Remove(permission);
        }
    }
}
