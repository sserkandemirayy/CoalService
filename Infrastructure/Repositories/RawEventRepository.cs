using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class RawEventRepository : IRawEventRepository
{
    private readonly AppDbContext _db;

    public RawEventRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<RawEvent?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.RawEvents.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<RawEvent?> GetByExternalEventIdAsync(Guid externalEventId, CancellationToken ct = default)
        => await _db.RawEvents.FirstOrDefaultAsync(x => x.ExternalEventId == externalEventId, ct);

    public async Task<bool> ExistsByExternalEventIdAsync(Guid externalEventId, CancellationToken ct = default)
        => await _db.RawEvents.AnyAsync(x => x.ExternalEventId == externalEventId, ct);

    public async Task AddAsync(RawEvent rawEvent, CancellationToken ct = default)
        => await _db.RawEvents.AddAsync(rawEvent, ct);

    public Task UpdateAsync(RawEvent rawEvent, CancellationToken ct = default)
    {
        _db.RawEvents.Update(rawEvent);
        return Task.CompletedTask;
    }

    public async Task<(IReadOnlyList<RawEvent> Items, int Total)> GetPagedAsync(
        string? eventType,
        string? tagExternalId,
        string? anchorExternalId,
        RawEventProcessingStatus? processingStatus,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = _db.RawEvents.AsQueryable();

        if (!string.IsNullOrWhiteSpace(eventType))
            query = query.Where(x => x.EventType == eventType);

        if (!string.IsNullOrWhiteSpace(tagExternalId))
            query = query.Where(x => x.TagExternalId == tagExternalId);

        if (!string.IsNullOrWhiteSpace(anchorExternalId))
            query = query.Where(x => x.AnchorExternalId == anchorExternalId);

        if (processingStatus.HasValue)
            query = query.Where(x => x.ProcessingStatus == processingStatus.Value);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(x => x.EventTimestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<RawEvent?> GetLatestByTagIdAsync(Guid tagId, CancellationToken ct = default)
    {
        var tagIdText = tagId.ToString();
        return await _db.RawEvents
            .Where(x => x.TagExternalId == tagIdText)
            .OrderByDescending(x => x.EventTimestamp)
            .FirstOrDefaultAsync(ct);
    }

    public IQueryable<RawEvent> Query() => _db.RawEvents.AsQueryable();
}