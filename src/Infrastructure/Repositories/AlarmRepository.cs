using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AlarmRepository : IAlarmRepository
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public AlarmRepository(
        AppDbContext db,
        ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<Alarm>> GetAllAsync(
        AlarmStatus? status = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken ct = default)
    {
        var query = ApplyScope(
            _db.Alarms.AsNoTracking());

        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);

        if (startDate.HasValue)
            query = query.Where(x => x.StartedAt >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(x => x.StartedAt <= endDate.Value);

        return await query
            .OrderByDescending(x => x.StartedAt)
            .ToListAsync(ct);
    }

    public async Task<Alarm?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default)
    {
        return await ApplyScope(_db.Alarms)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<IEnumerable<Alarm>> GetActiveAlarmsAsync(
        CancellationToken ct = default)
    {
        return await ApplyScope(
                _db.Alarms.AsNoTracking())
            .Where(x =>
                x.Status == AlarmStatus.Active ||
                x.Status == AlarmStatus.Acknowledged)
            .OrderByDescending(x => x.StartedAt)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Alarm>> GetByTagIdAsync(
        Guid tagId,
        CancellationToken ct = default)
    {
        return await ApplyScope(
                _db.Alarms.AsNoTracking())
            .Where(x =>
                x.TagId == tagId ||
                x.PeerTagId == tagId)
            .OrderByDescending(x => x.StartedAt)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Alarm>> GetByAnchorIdAsync(
        Guid anchorId,
        CancellationToken ct = default)
    {
        return await ApplyScope(
                _db.Alarms.AsNoTracking())
            .Where(x => x.AnchorId == anchorId)
            .OrderByDescending(x => x.StartedAt)
            .ToListAsync(ct);
    }

    public async Task<bool> HasActiveAlarmAsync(
        AlarmType alarmType,
        Guid? tagId = null,
        Guid? peerTagId = null,
        Guid? anchorId = null,
        CancellationToken ct = default)
    {
        var query = _db.Alarms
            .AsNoTracking()
            .Where(x =>
                x.AlarmType == alarmType &&
                (x.Status == AlarmStatus.Active ||
                 x.Status == AlarmStatus.Acknowledged));

        if (tagId.HasValue)
            query = query.Where(x => x.TagId == tagId.Value);

        if (peerTagId.HasValue)
        {
            query = query.Where(
                x => x.PeerTagId == peerTagId.Value);
        }

        if (anchorId.HasValue)
            query = query.Where(x => x.AnchorId == anchorId.Value);

        return await query.AnyAsync(ct);
    }

    public async Task AddAsync(
        Alarm alarm,
        CancellationToken ct = default)
    {
        await _db.Alarms.AddAsync(alarm, ct);
    }

    public Task UpdateAsync(
        Alarm alarm,
        CancellationToken ct = default)
    {
        _db.Alarms.Update(alarm);
        return Task.CompletedTask;
    }

    public IQueryable<Alarm> Query()
    {
        return _db.Alarms.AsQueryable();
    }

    private IQueryable<Alarm> ApplyScope(
        IQueryable<Alarm> query)
    {
        if (HasUnrestrictedScope())
            return query;

        var companyIds =
            _currentUser.GetCurrentUserCompanyIds();

        var branchIds =
            _currentUser.GetCurrentUserBranchIds();

        return query.Where(x =>
            (
                x.UserId.HasValue &&
                (
                    x.User!.UserCompanies.Any(uc =>
                        companyIds.Contains(uc.CompanyId)) ||
                    x.User.UserBranches.Any(ub =>
                        branchIds.Contains(ub.BranchId))
                )
            )
            ||
            (
                x.TagId.HasValue &&
                x.Tag!.Assignments.Any(a =>
                    a.UnassignedAt == null &&
                    (
                        a.User.UserCompanies.Any(uc =>
                            companyIds.Contains(uc.CompanyId)) ||
                        a.User.UserBranches.Any(ub =>
                            branchIds.Contains(ub.BranchId))
                    ))
            )
            ||
            (
                x.PeerTagId.HasValue &&
                x.PeerTag!.Assignments.Any(a =>
                    a.UnassignedAt == null &&
                    (
                        a.User.UserCompanies.Any(uc =>
                            companyIds.Contains(uc.CompanyId)) ||
                        a.User.UserBranches.Any(ub =>
                            branchIds.Contains(ub.BranchId))
                    ))
            )
            ||
            (
                x.AnchorId.HasValue &&
                (
                    (
                        x.Anchor!.CompanyId.HasValue &&
                        companyIds.Contains(
                            x.Anchor.CompanyId.Value)
                    )
                    ||
                    (
                        x.Anchor.BranchId.HasValue &&
                        branchIds.Contains(
                            x.Anchor.BranchId.Value)
                    )
                )
            ));
    }

    private bool HasUnrestrictedScope()
    {
        return _currentUser.IsSystemUser() ||
               _currentUser.GetRoles().Any(x =>
                   x.Equals(
                       "super_admin",
                       StringComparison.OrdinalIgnoreCase));
    }
}