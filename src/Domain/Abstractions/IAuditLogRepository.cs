using Domain.Entities;

namespace Domain.Abstractions;

public interface IAuditLogRepository
{
    Task AddAsync(AuditLog log, CancellationToken ct = default);

    Task<(List<AuditLog> Logs, int Total)> GetPagedAsync(
        Guid? userId,
        string? action,
        DateTime? from,
        DateTime? to,
        int page,
        int pageSize,
        CancellationToken ct = default);
}
