using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CommandRequestRepository : ICommandRequestRepository
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CommandRequestRepository(AppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<CommandRequest?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await ApplyScope(_db.CommandRequests).FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task AddAsync(CommandRequest commandRequest, CancellationToken ct = default)
        => await _db.CommandRequests.AddAsync(commandRequest, ct);

    public Task UpdateAsync(CommandRequest commandRequest, CancellationToken ct = default)
    {
        _db.CommandRequests.Update(commandRequest);
        return Task.CompletedTask;
    }

    public async Task<(IReadOnlyList<CommandRequest> Items, int Total)> GetPagedAsync(
        RtlsCommandType? commandType,
        RtlsCommandStatus? status,
        RtlsCommandTargetType? targetType,
        Guid? tagId,
        Guid? anchorId,
        Guid? requestedByUserId,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = ApplyScope(_db.CommandRequests);

        if (commandType.HasValue)
            query = query.Where(x => x.CommandType == commandType.Value);

        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);

        if (targetType.HasValue)
            query = query.Where(x => x.TargetType == targetType.Value);

        if (tagId.HasValue)
            query = query.Where(x => x.TagId == tagId.Value);

        if (anchorId.HasValue)
            query = query.Where(x => x.AnchorId == anchorId.Value);

        if (requestedByUserId.HasValue)
            query = query.Where(x => x.RequestedByUserId == requestedByUserId.Value);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(x => x.RequestedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<(IReadOnlyList<CommandRequest> Items, int Total)> GetPagedByTagIdAsync(
        Guid tagId,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = ApplyScope(_db.CommandRequests).Where(x => x.TagId == tagId);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(x => x.RequestedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<(IReadOnlyList<CommandRequest> Items, int Total)> GetPagedByAnchorIdAsync(
        Guid anchorId,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = ApplyScope(_db.CommandRequests).Where(x => x.AnchorId == anchorId);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(x => x.RequestedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public IQueryable<CommandRequest> Query() => _db.CommandRequests.AsQueryable();

    private IQueryable<CommandRequest> ApplyScope(IQueryable<CommandRequest> query)
    {
        if (HasUnrestrictedScope())
            return query;

        var companyIds = _currentUser.GetCurrentUserCompanyIds();
        var branchIds = _currentUser.GetCurrentUserBranchIds();

        return query.Where(x =>
            x.RequestedByUser.UserCompanies.Any(uc => companyIds.Contains(uc.CompanyId)) ||
            x.RequestedByUser.UserBranches.Any(ub => branchIds.Contains(ub.BranchId)) ||
            (x.TagId.HasValue && x.Tag!.Assignments.Any(a =>
                a.UnassignedAt == null &&
                (a.User.UserCompanies.Any(uc => companyIds.Contains(uc.CompanyId)) ||
                 a.User.UserBranches.Any(ub => branchIds.Contains(ub.BranchId))))) ||
            (x.AnchorId.HasValue &&
                ((x.Anchor!.CompanyId.HasValue && companyIds.Contains(x.Anchor.CompanyId.Value)) ||
                 (x.Anchor.BranchId.HasValue && branchIds.Contains(x.Anchor.BranchId.Value)))));
    }

    private bool HasUnrestrictedScope()
        => _currentUser.IsSystemUser() ||
           _currentUser.GetRoles().Any(x => x.Equals("super_admin", StringComparison.OrdinalIgnoreCase));
}
