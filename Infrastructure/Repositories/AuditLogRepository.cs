using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly AppDbContext _db;
    public AuditLogRepository(AppDbContext db) => _db = db;

    public async Task AddAsync(AuditLog log, CancellationToken ct = default)
    {
        await _db.AuditLogs.AddAsync(log, ct);
    }

    public async Task<(List<AuditLog> Logs, int Total)> GetPagedAsync(
        Guid? userId,
        string? action,
        DateTime? from,
        DateTime? to,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = _db.AuditLogs.AsQueryable();

        if (userId.HasValue)
            query = query.Where(x => x.UserId == userId);

        if (!string.IsNullOrWhiteSpace(action))
            query = query.Where(x => x.Action.Contains(action));

        if (from.HasValue)
            query = query.Where(x => x.Timestamp >= from);

        if (to.HasValue)
            query = query.Where(x => x.Timestamp <= to);

        var total = await query.CountAsync(ct);

        var logs = await query
            .OrderByDescending(x => x.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (logs, total);
    }
}
