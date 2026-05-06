using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CompanyRepository : ICompanyRepository
{
    private readonly AppDbContext _db;
    public CompanyRepository(AppDbContext db) => _db = db;

    public async Task<Company?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.Companies
            .Include(c => c.Branches)
            .FirstOrDefaultAsync(c => c.Id == id && c.DeletedAt == null, ct);

    public async Task<IEnumerable<Company>> GetAllAsync(CancellationToken ct = default)
        => await _db.Companies
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
}