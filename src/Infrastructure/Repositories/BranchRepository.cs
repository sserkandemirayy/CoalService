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
        var companyIds = _currentUser.GetCurrentUserCompanyIds();
        return await _db.Branches
            .Include(b => b.Company)
            .Where(b => companyIds.Contains(b.CompanyId))
            .FirstOrDefaultAsync(b => b.Id == id && b.DeletedAt == null, ct);
    }

    public async Task<IEnumerable<Branch>> GetByCompanyIdAsync(Guid companyId, CancellationToken ct = default)
    {
        var companyIds = _currentUser.GetCurrentUserCompanyIds();
        if (!companyIds.Contains(companyId))
            return Enumerable.Empty<Branch>();

        return await _db.Branches
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
}