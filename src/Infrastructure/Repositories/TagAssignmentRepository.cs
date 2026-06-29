using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TagAssignmentRepository : ITagAssignmentRepository
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public TagAssignmentRepository(AppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<TagAssignment?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await ApplyScope(_db.TagAssignments).FirstOrDefaultAsync(x => x.Id == id, ct);


    public async Task<TagAssignment?> GetActiveByTagIdAsync(Guid tagId, CancellationToken ct = default)
     => await ApplyScope(_db.TagAssignments)
         .Include(x => x.User)
         .FirstOrDefaultAsync(x => x.TagId == tagId && x.UnassignedAt == null, ct);

    public async Task<TagAssignment?> GetActiveByUserIdAsync(Guid userId, CancellationToken ct = default)
        => await ApplyScope(_db.TagAssignments)
            .FirstOrDefaultAsync(x => x.UserId == userId && x.UnassignedAt == null, ct);

    public async Task<IEnumerable<TagAssignment>> GetAssignmentsByUserIdAsync(Guid userId, CancellationToken ct = default)
        => await ApplyScope(_db.TagAssignments)
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.AssignedAt)
            .ToListAsync(ct);

    public async Task<IEnumerable<TagAssignment>> GetAssignmentsByTagIdAsync(Guid tagId, CancellationToken ct = default)
        => await ApplyScope(_db.TagAssignments)
            .Where(x => x.TagId == tagId)
            .OrderByDescending(x => x.AssignedAt)
            .ToListAsync(ct);

    public async Task AddAsync(TagAssignment assignment, CancellationToken ct = default)
        => await _db.TagAssignments.AddAsync(assignment, ct);

    public Task UpdateAsync(TagAssignment assignment, CancellationToken ct = default)
    {
        _db.TagAssignments.Update(assignment);
        return Task.CompletedTask;
    }

    public IQueryable<TagAssignment> Query() => _db.TagAssignments.AsQueryable();

    private IQueryable<TagAssignment> ApplyScope(IQueryable<TagAssignment> query)
    {
        if (HasUnrestrictedScope())
            return query;

        var currentUserId = _currentUser.GetCurrentUserId();
        var companyIds = _currentUser.GetCurrentUserCompanyIds();
        var branchIds = _currentUser.GetCurrentUserBranchIds();

        return query.Where(x =>
            x.UserId == currentUserId ||
            x.User.UserCompanies.Any(uc => companyIds.Contains(uc.CompanyId)) ||
            x.User.UserBranches.Any(ub => branchIds.Contains(ub.BranchId)) ||
            x.Tag.CreatedBy == currentUserId ||
            x.Tag.Assignments.Any(a =>
                a.UnassignedAt == null &&
                (a.User.UserCompanies.Any(uc => companyIds.Contains(uc.CompanyId)) ||
                 a.User.UserBranches.Any(ub => branchIds.Contains(ub.BranchId)))));
    }

    private bool HasUnrestrictedScope()
        => _currentUser.IsSystemUser() ||
           _currentUser.GetRoles().Any(x => x.Equals("super_admin", StringComparison.OrdinalIgnoreCase));
}
