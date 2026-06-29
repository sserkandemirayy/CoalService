using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TagRepository : ITagRepository
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public TagRepository(AppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Tag?> GetByIdAsync(Guid id, CancellationToken ct = default)
    => await ApplyScope(_db.Tags)
        .Include(x => x.Assignments.Where(a => a.UnassignedAt == null))
        .ThenInclude(a => a.User)
        .FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<Tag?> GetByExternalIdAsync(string externalId, CancellationToken ct = default)
        => await _db.Tags.FirstOrDefaultAsync(x => x.ExternalId == externalId, ct);

    public async Task<Tag?> GetScopedByExternalIdAsync(string externalId, CancellationToken ct = default)
        => await ApplyScope(_db.Tags).FirstOrDefaultAsync(x => x.ExternalId == externalId, ct);

    public async Task<Tag?> GetByCodeAsync(string code, CancellationToken ct = default)
        => await _db.Tags.FirstOrDefaultAsync(x => x.Code == code, ct);

    public async Task AddAsync(Tag tag, CancellationToken ct = default)
        => await _db.Tags.AddAsync(tag, ct);

    public Task UpdateAsync(Tag tag, CancellationToken ct = default)
    {
        _db.Tags.Update(tag);
        return Task.CompletedTask;
    }

    public async Task<(IReadOnlyList<Tag> Items, int Total)> GetPagedAsync(
        string? search,
        TagStatus? status,
        TagType? tagType,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        //var query = _db.Tags.AsQueryable();

        var query = ApplyScope(_db.Tags)
            .Include(x => x.Assignments
                .Where(a => a.UnassignedAt == null))
            .ThenInclude(a => a.User)
            .AsQueryable();

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

        if (tagType.HasValue)
            query = query.Where(x => x.TagType == tagType.Value);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderBy(x => x.Code)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<int> CountAsync(CancellationToken ct = default)
        => await ApplyScope(_db.Tags).CountAsync(ct);

    public async Task<int> CountByStatusAsync(TagStatus status, CancellationToken ct = default)
        => await ApplyScope(_db.Tags).CountAsync(x => x.Status == status, ct);

    public IQueryable<Tag> Query() => _db.Tags.AsQueryable();

    private IQueryable<Tag> ApplyScope(IQueryable<Tag> query)
    {
        if (HasUnrestrictedScope())
            return query;

        var companyIds = _currentUser.GetCurrentUserCompanyIds();
        var branchIds = _currentUser.GetCurrentUserBranchIds();
        var currentUserId = _currentUser.GetCurrentUserId();

        return query.Where(x =>
            x.CreatedBy == currentUserId ||
            x.Assignments.Any(a =>
                a.UnassignedAt == null &&
                (a.User.UserCompanies.Any(uc => companyIds.Contains(uc.CompanyId)) ||
                 a.User.UserBranches.Any(ub => branchIds.Contains(ub.BranchId)))));
    }

    private bool HasUnrestrictedScope()
        => _currentUser.IsSystemUser() ||
           _currentUser.GetRoles().Any(x => x.Equals("super_admin", StringComparison.OrdinalIgnoreCase));
}
