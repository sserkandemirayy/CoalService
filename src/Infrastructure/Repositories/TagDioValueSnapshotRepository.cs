using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TagDioValueSnapshotRepository : ITagDioValueSnapshotRepository
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public TagDioValueSnapshotRepository(AppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<TagDioValueSnapshot?> GetByTagIdAndPinAsync(Guid tagId, int pin, CancellationToken ct = default)
        => await Scoped().FirstOrDefaultAsync(x => x.TagId == tagId && x.Pin == pin, ct);

    public async Task<IReadOnlyList<TagDioValueSnapshot>> GetByTagIdAsync(Guid tagId, CancellationToken ct = default)
        => await Scoped()
            .Where(x => x.TagId == tagId)
            .OrderBy(x => x.Pin)
            .ToListAsync(ct);

    public async Task AddAsync(TagDioValueSnapshot entity, CancellationToken ct = default)
        => await _db.TagDioValueSnapshots.AddAsync(entity, ct);

    public Task UpdateAsync(TagDioValueSnapshot entity, CancellationToken ct = default)
    {
        _db.TagDioValueSnapshots.Update(entity);
        return Task.CompletedTask;
    }

    private IQueryable<TagDioValueSnapshot> Scoped()
    {
        var tags = RepositoryScope.Tags(_db, _currentUser);
        return _db.TagDioValueSnapshots.Where(x => tags.Any(t => t.Id == x.TagId));
    }

    public IQueryable<TagDioValueSnapshot> Query() => _db.TagDioValueSnapshots.AsQueryable();
}
