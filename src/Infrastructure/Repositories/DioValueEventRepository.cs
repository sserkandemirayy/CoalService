using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class DioValueEventRepository : IDioValueEventRepository
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public DioValueEventRepository(AppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<DioValueEvent?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await Scoped().FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task AddAsync(DioValueEvent entity, CancellationToken ct = default)
        => await _db.DioValueEvents.AddAsync(entity, ct);

    public async Task<(IReadOnlyList<DioValueEvent> Items, int Total)> GetPagedByTagIdAsync(
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

    private IQueryable<DioValueEvent> Scoped()
    {
        var tags = RepositoryScope.Tags(_db, _currentUser);
        return _db.DioValueEvents.Where(x => tags.Any(t => t.Id == x.TagId));
    }

    public IQueryable<DioValueEvent> Query() => _db.DioValueEvents.AsQueryable();
}