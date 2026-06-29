using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories;

internal static class RepositoryScope
{
    public static bool HasUnrestrictedScope(ICurrentUserService currentUser)
        => currentUser.IsSystemUser() ||
           currentUser.GetRoles().Any(x => x.Equals("super_admin", StringComparison.OrdinalIgnoreCase));

    public static IQueryable<Tag> Tags(AppDbContext db, ICurrentUserService currentUser)
    {
        var query = db.Tags.AsQueryable();
        if (HasUnrestrictedScope(currentUser))
            return query;

        var currentUserId = currentUser.GetCurrentUserId();
        var companyIds = currentUser.GetCurrentUserCompanyIds();
        var branchIds = currentUser.GetCurrentUserBranchIds();

        return query.Where(x =>
            x.CreatedBy == currentUserId ||
            x.Assignments.Any(a =>
                a.UnassignedAt == null &&
                (a.User.UserCompanies.Any(uc => companyIds.Contains(uc.CompanyId)) ||
                 a.User.UserBranches.Any(ub => branchIds.Contains(ub.BranchId)))));
    }

    public static IQueryable<Anchor> Anchors(AppDbContext db, ICurrentUserService currentUser)
    {
        var query = db.Anchors.AsQueryable();
        if (HasUnrestrictedScope(currentUser))
            return query;

        var companyIds = currentUser.GetCurrentUserCompanyIds();
        var branchIds = currentUser.GetCurrentUserBranchIds();

        return query.Where(x =>
            (x.CompanyId.HasValue && companyIds.Contains(x.CompanyId.Value)) ||
            (x.BranchId.HasValue && branchIds.Contains(x.BranchId.Value)));
    }

    public static IQueryable<RawEvent> RawEvents(AppDbContext db, ICurrentUserService currentUser)
    {
        var query = db.RawEvents.AsQueryable();
        if (HasUnrestrictedScope(currentUser))
            return query;

        var tags = Tags(db, currentUser);
        var anchors = Anchors(db, currentUser);

        return query.Where(x =>
            (x.TagExternalId != null && tags.Any(t => t.ExternalId == x.TagExternalId)) ||
            (x.AnchorExternalId != null && anchors.Any(a => a.ExternalId == x.AnchorExternalId)));
    }
}
