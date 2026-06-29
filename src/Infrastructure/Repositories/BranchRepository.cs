using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class BranchRepository : IBranchRepository
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public BranchRepository(AppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Branch?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await ApplyScope(_db.Branches)
            .Include(b => b.Company)
            .FirstOrDefaultAsync(b => b.Id == id && b.DeletedAt == null, ct);
    }

    public async Task<IEnumerable<Branch>> GetByCompanyIdAsync(Guid companyId, CancellationToken ct = default)
    {
        return await ApplyScope(_db.Branches)
            .Where(b => b.CompanyId == companyId && b.DeletedAt == null)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Branch branch, CancellationToken ct = default)
        => await _db.Branches.AddAsync(branch, ct);

    public Task UpdateAsync(Branch branch, CancellationToken ct = default)
    {
        _db.Branches.Update(branch);
        return Task.CompletedTask;
    }

    private IQueryable<Branch> ApplyScope(IQueryable<Branch> query)
    {
        if (HasUnrestrictedScope())
            return query;

        var companyIds = _currentUser.GetCurrentUserCompanyIds();
        return query.Where(x => companyIds.Contains(x.CompanyId));
    }

    private bool HasUnrestrictedScope()
        => _currentUser.IsSystemUser() ||
           _currentUser.GetRoles().Any(x => x.Equals("super_admin", StringComparison.OrdinalIgnoreCase));
}
