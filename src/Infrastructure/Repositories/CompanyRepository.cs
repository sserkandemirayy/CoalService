using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CompanyRepository : ICompanyRepository
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CompanyRepository(AppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Company?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await ApplyScope(_db.Companies)
            .Include(c => c.Branches)
            .FirstOrDefaultAsync(c => c.Id == id && c.DeletedAt == null, ct);

    public async Task<IEnumerable<Company>> GetAllAsync(CancellationToken ct = default)
        => await ApplyScope(_db.Companies)
            .Include(c => c.Branches)
            .Where(c => c.DeletedAt == null)
            .ToListAsync(ct);

    public async Task AddAsync(Company company, CancellationToken ct = default)
        => await _db.Companies.AddAsync(company, ct);

    public Task UpdateAsync(Company company, CancellationToken ct = default)
    {
        _db.Companies.Update(company);
        return Task.CompletedTask;
    }

    private IQueryable<Company> ApplyScope(IQueryable<Company> query)
    {
        if (HasUnrestrictedScope())
            return query;

        var companyIds = _currentUser.GetCurrentUserCompanyIds();
        return query.Where(x => companyIds.Contains(x.Id));
    }

    private bool HasUnrestrictedScope()
        => _currentUser.IsSystemUser() ||
           _currentUser.GetRoles().Any(x => x.Equals("super_admin", StringComparison.OrdinalIgnoreCase));
}
