using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AnchorRepository : IAnchorRepository
{
    private readonly AppDbContext _db;

    public AnchorRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Anchor?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.Anchors.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<Anchor?> GetByExternalIdAsync(string externalId, CancellationToken ct = default)
        => await _db.Anchors.FirstOrDefaultAsync(x => x.ExternalId == externalId, ct);

    public async Task<Anchor?> GetByCodeAsync(string code, CancellationToken ct = default)
        => await _db.Anchors.FirstOrDefaultAsync(x => x.Code == code, ct);

    public async Task AddAsync(Anchor anchor, CancellationToken ct = default)
        => await _db.Anchors.AddAsync(anchor, ct);

    public Task UpdateAsync(Anchor anchor, CancellationToken ct = default)
    {
        _db.Anchors.Update(anchor);
        return Task.CompletedTask;
    }

    public async Task<(IReadOnlyList<Anchor> Items, int Total)> GetPagedAsync(
        string? search,
        AnchorStatus? status,
        Guid? companyId,
        Guid? branchId,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = _db.Anchors.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();
            query = query.Where(x =>
                x.Code.Contains(search) ||
                x.ExternalId.Contains(search) ||
                (x.Name != null && x.Name.Contains(search)));
        }

        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);

        if (companyId.HasValue)
            query = query.Where(x => x.CompanyId == companyId.Value);

        if (branchId.HasValue)
            query = query.Where(x => x.BranchId == branchId.Value);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderBy(x => x.Code)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<int> CountAsync(CancellationToken ct = default)
        => await _db.Anchors.CountAsync(ct);

    public async Task<int> CountByStatusAsync(AnchorStatus status, CancellationToken ct = default)
        => await _db.Anchors.CountAsync(x => x.Status == status, ct);

    public IQueryable<Anchor> Query() => _db.Anchors.AsQueryable();
}