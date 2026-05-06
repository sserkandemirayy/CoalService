using Domain.Entities;

namespace Domain.Abstractions;

public interface ICommandStatusHistoryRepository
{
    Task AddAsync(CommandStatusHistory history, CancellationToken ct = default);
    Task<IReadOnlyList<CommandStatusHistory>> GetByCommandRequestIdAsync(Guid commandRequestId, CancellationToken ct = default);
    IQueryable<CommandStatusHistory> Query();
}