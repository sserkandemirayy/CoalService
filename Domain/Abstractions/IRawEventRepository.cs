using Domain.Entities;
using Domain.Enums;

namespace Domain.Abstractions;

public interface IRawEventRepository
{
    Task<RawEvent?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<RawEvent?> GetByExternalEventIdAsync(Guid externalEventId, CancellationToken ct = default);
    Task<bool> ExistsByExternalEventIdAsync(Guid externalEventId, CancellationToken ct = default);
    Task AddAsync(RawEvent rawEvent, CancellationToken ct = default);
    Task UpdateAsync(RawEvent rawEvent, CancellationToken ct = default);

    Task<(IReadOnlyList<RawEvent> Items, int Total)> GetPagedAsync(
        string? eventType,
        string? tagExternalId,
        string? anchorExternalId,
        RawEventProcessingStatus? processingStatus,
        int page,
        int pageSize,
        CancellationToken ct = default);

    Task<RawEvent?> GetLatestByTagIdAsync(Guid tagId, CancellationToken ct = default);

    IQueryable<RawEvent> Query();
}