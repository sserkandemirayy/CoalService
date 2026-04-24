using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CommandStatusHistoryRepository : ICommandStatusHistoryRepository
{
    private readonly AppDbContext _db;

    public CommandStatusHistoryRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(CommandStatusHistory history, CancellationToken ct = default)
        => await _db.CommandStatusHistories.AddAsync(history, ct);

    public async Task<IReadOnlyList<CommandStatusHistory>> GetByCommandRequestIdAsync(Guid commandRequestId, CancellationToken ct = default)
        => await _db.CommandStatusHistories
            .Where(x => x.CommandRequestId == commandRequestId)
            .OrderBy(x => x.ChangedAt)
            .ToListAsync(ct);

    public IQueryable<CommandStatusHistory> Query() => _db.CommandStatusHistories.AsQueryable();
}