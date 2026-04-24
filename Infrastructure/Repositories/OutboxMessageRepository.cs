using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class OutboxMessageRepository : IOutboxMessageRepository
{
    private readonly AppDbContext _db;

    public OutboxMessageRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<OutboxMessage?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.OutboxMessages.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task AddAsync(OutboxMessage message, CancellationToken ct = default)
        => await _db.OutboxMessages.AddAsync(message, ct);

    public Task UpdateAsync(OutboxMessage message, CancellationToken ct = default)
    {
        _db.OutboxMessages.Update(message);
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<OutboxMessage>> GetPendingAsync(int take, CancellationToken ct = default)
        => await _db.OutboxMessages
            .Where(x => x.Status == OutboxMessageStatus.Pending)
            .OrderBy(x => x.OccurredAt)
            .Take(take)
            .ToListAsync(ct);

    public async Task<(IReadOnlyList<OutboxMessage> Items, int Total)> GetPagedAsync(
        OutboxMessageStatus? status,
        string? aggregateType,
        string? messageType,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = _db.OutboxMessages.AsQueryable();

        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(aggregateType))
            query = query.Where(x => x.AggregateType == aggregateType);

        if (!string.IsNullOrWhiteSpace(messageType))
            query = query.Where(x => x.MessageType == messageType);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(x => x.OccurredAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public IQueryable<OutboxMessage> Query() => _db.OutboxMessages.AsQueryable();
}