using Domain.Entities;
using Domain.Enums;

namespace Domain.Abstractions;

public interface IOutboxMessageRepository
{
    Task<OutboxMessage?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(OutboxMessage message, CancellationToken ct = default);
    Task UpdateAsync(OutboxMessage message, CancellationToken ct = default);

    Task<IReadOnlyList<OutboxMessage>> GetPendingAsync(int take, CancellationToken ct = default);

    Task<(IReadOnlyList<OutboxMessage> Items, int Total)> GetPagedAsync(
        OutboxMessageStatus? status,
        string? aggregateType,
        string? messageType,
        int page,
        int pageSize,
        CancellationToken ct = default);

    IQueryable<OutboxMessage> Query();
}