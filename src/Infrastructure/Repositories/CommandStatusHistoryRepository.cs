using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CommandStatusHistoryRepository : ICommandStatusHistoryRepository
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CommandStatusHistoryRepository(AppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task AddAsync(CommandStatusHistory history, CancellationToken ct = default)
        => await _db.CommandStatusHistories.AddAsync(history, ct);

    public async Task<IReadOnlyList<CommandStatusHistory>> GetByCommandRequestIdAsync(Guid commandRequestId, CancellationToken ct = default)
        => await ApplyScope(_db.CommandStatusHistories)
            .Where(x => x.CommandRequestId == commandRequestId)
            .OrderBy(x => x.ChangedAt)
            .ToListAsync(ct);

    public IQueryable<CommandStatusHistory> Query() => _db.CommandStatusHistories.AsQueryable();

    private IQueryable<CommandStatusHistory> ApplyScope(IQueryable<CommandStatusHistory> query)
    {
        if (HasUnrestrictedScope())
            return query;

        var companyIds = _currentUser.GetCurrentUserCompanyIds();
        var branchIds = _currentUser.GetCurrentUserBranchIds();

        return query.Where(x =>
            x.CommandRequest.RequestedByUser.UserCompanies.Any(uc => companyIds.Contains(uc.CompanyId)) ||
            x.CommandRequest.RequestedByUser.UserBranches.Any(ub => branchIds.Contains(ub.BranchId)) ||
            (x.CommandRequest.TagId.HasValue && x.CommandRequest.Tag!.Assignments.Any(a =>
                a.UnassignedAt == null &&
                (a.User.UserCompanies.Any(uc => companyIds.Contains(uc.CompanyId)) ||
                 a.User.UserBranches.Any(ub => branchIds.Contains(ub.BranchId))))) ||
            (x.CommandRequest.AnchorId.HasValue &&
                ((x.CommandRequest.Anchor!.CompanyId.HasValue && companyIds.Contains(x.CommandRequest.Anchor.CompanyId.Value)) ||
                 (x.CommandRequest.Anchor.BranchId.HasValue && branchIds.Contains(x.CommandRequest.Anchor.BranchId.Value)))));
    }

    private bool HasUnrestrictedScope()
        => _currentUser.IsSystemUser() ||
           _currentUser.GetRoles().Any(x => x.Equals("super_admin", StringComparison.OrdinalIgnoreCase));
}
