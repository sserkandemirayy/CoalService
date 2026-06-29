using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TagDataEventRepository : ITagDataEventRepository
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public TagDataEventRepository(AppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<TagDataEvent?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await Scoped().FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task AddAsync(TagDataEvent tagDataEvent, CancellationToken ct = default)
        => await _db.TagDataEvents.AddAsync(tagDataEvent, ct);

    public async Task<(IReadOnlyList<TagDataEvent> Items, int Total)> GetPagedByTagIdAsync(
        Guid tagId,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = Scoped().Where(x => x.TagId == tagId);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(x => x.EventTimestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    private IQueryable<TagDataEvent> Scoped()
    {
        var tags = RepositoryScope.Tags(_db, _currentUser);
        return _db.TagDataEvents.Where(x => tags.Any(t => t.Id == x.TagId));
    }

    public IQueryable<TagDataEvent> Query() => _db.TagDataEvents.AsQueryable();
}