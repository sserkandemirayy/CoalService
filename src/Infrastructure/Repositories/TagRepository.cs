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

    public TagRepository(
        AppDbContext db,
        ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Tag?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default)
    {
        return await ApplyScope(_db.Tags)
            .Include(x => x.Assignments
                .Where(a => a.UnassignedAt == null))
                .ThenInclude(a => a.User)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                ct);
    }

    /// <summary>
    /// Event processing tarafýndan kullanýlabilir.
    /// Scope uygulanmaz çünkü dýþ sistemden ExternalId ile
    /// gelen Tag'in bulunmasý gerekir.
    /// </summary>
    public async Task<Tag?> GetByExternalIdAsync(
        string externalId,
        CancellationToken ct = default)
    {
        return await _db.Tags
            .FirstOrDefaultAsync(
                x => x.ExternalId == externalId,
                ct);
    }

    public async Task<Tag?> GetScopedByExternalIdAsync(
        string externalId,
        CancellationToken ct = default)
    {
        return await ApplyScope(_db.Tags)
            .FirstOrDefaultAsync(
                x => x.ExternalId == externalId,
                ct);
    }

    public async Task<Tag?> GetByCodeAsync(
        string code,
        CancellationToken ct = default)
    {
        return await _db.Tags
            .FirstOrDefaultAsync(
                x => x.Code == code,
                ct);
    }

    public async Task AddAsync(
        Tag tag,
        CancellationToken ct = default)
    {
        await _db.Tags.AddAsync(tag, ct);
    }

    public Task UpdateAsync(
        Tag tag,
        CancellationToken ct = default)
    {
        _db.Tags.Update(tag);
        return Task.CompletedTask;
    }

    public async Task<(IReadOnlyList<Tag> Items, int Total)> GetPagedAsync(
        string? search,
        TagStatus? status,
        TagType? tagType,
        Guid? companyId,
        Guid? branchId,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        if (page < 1)
            page = 1;

        if (pageSize < 1)
            pageSize = 20;

        if (pageSize > 500)
            pageSize = 500;

        var query = ApplyScope(_db.Tags)
            .Include(x => x.Assignments
                .Where(a => a.UnassignedAt == null))
                .ThenInclude(a => a.User)
            .AsQueryable();

        // ============================================================
        // SEARCH
        // ============================================================

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim().ToLower();

            query = query.Where(x =>
                x.Code.ToLower().Contains(normalizedSearch) ||
                x.ExternalId.ToLower().Contains(normalizedSearch) ||
                (x.Name != null &&
                 x.Name.ToLower().Contains(normalizedSearch)) ||
                (x.SerialNumber != null &&
                 x.SerialNumber.ToLower().Contains(normalizedSearch)));
        }

        // ============================================================
        // STATUS
        // ============================================================

        if (status.HasValue)
        {
            query = query.Where(
                x => x.Status == status.Value);
        }

        // ============================================================
        // TAG TYPE
        // ============================================================

        if (tagType.HasValue)
        {
            query = query.Where(
                x => x.TagType == tagType.Value);
        }

        // ============================================================
        // COMPANY
        // ============================================================

        if (companyId.HasValue &&
            companyId.Value != Guid.Empty)
        {
            query = query.Where(
                x => x.CompanyId == companyId.Value);
        }

        // ============================================================
        // BRANCH
        // ============================================================

        if (branchId.HasValue &&
            branchId.Value != Guid.Empty)
        {
            query = query.Where(
                x => x.BranchId == branchId.Value);
        }

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderBy(x => x.Code)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<int> CountAsync(
        CancellationToken ct = default)
    {
        return await ApplyScope(_db.Tags)
            .CountAsync(ct);
    }

    public async Task<int> CountByStatusAsync(
        TagStatus status,
        CancellationToken ct = default)
    {
        return await ApplyScope(_db.Tags)
            .CountAsync(
                x => x.Status == status,
                ct);
    }

    public IQueryable<Tag> Query()
    {
        return _db.Tags.AsQueryable();
    }

    // ================================================================
    // SCOPE
    // ================================================================

    private IQueryable<Tag> ApplyScope(
        IQueryable<Tag> query)
    {
        if (HasUnrestrictedScope())
            return query;

        var companyIds = _currentUser
            .GetCurrentUserCompanyIds();

        var branchIds = _currentUser
            .GetCurrentUserBranchIds();

        var currentUserId = _currentUser
            .GetCurrentUserId();

        return query.Where(x =>

            // --------------------------------------------------------
            // Yeni sistem:
            // Tag doðrudan kullanýcýnýn yetkili olduðu þirkete baðlý.
            // --------------------------------------------------------

            (x.CompanyId.HasValue &&
             companyIds.Contains(x.CompanyId.Value))

            ||

            // --------------------------------------------------------
            // Tag doðrudan kullanýcýnýn yetkili olduðu þubeye baðlý.
            // --------------------------------------------------------

            (x.BranchId.HasValue &&
             branchIds.Contains(x.BranchId.Value))

            ||

            // --------------------------------------------------------
            // Legacy fallback:
            // Migration öncesinde Company / Branch bilgisi olmayan
            // eski Tag kayýtlarý tamamen görünmez olmasýn.
            // --------------------------------------------------------

            ((!x.CompanyId.HasValue &&
              !x.BranchId.HasValue)
             &&
             (
                 x.CreatedBy == currentUserId

                 ||

                 x.Assignments.Any(a =>
                     a.UnassignedAt == null &&
                     (
                         a.User.UserCompanies.Any(uc =>
                             companyIds.Contains(uc.CompanyId))

                         ||

                         a.User.UserBranches.Any(ub =>
                             branchIds.Contains(ub.BranchId))
                     ))
             ))
        );
    }

    private bool HasUnrestrictedScope()
    {
        return _currentUser.IsSystemUser()
               ||
               _currentUser
                   .GetRoles()
                   .Any(x =>
                       x.Equals(
                           "super_admin",
                           StringComparison.OrdinalIgnoreCase));
    }
}