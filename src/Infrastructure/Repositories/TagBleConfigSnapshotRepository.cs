using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TagBleConfigSnapshotRepository : ITagBleConfigSnapshotRepository
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserService _currentUser;
    public TagBleConfigSnapshotRepository(AppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<TagBleConfigSnapshot?> GetByTagIdAsync(Guid tagId, CancellationToken ct = default)
        => await Scoped().FirstOrDefaultAsync(x => x.TagId == tagId, ct);

    public async Task AddAsync(TagBleConfigSnapshot entity, CancellationToken ct = default)
        => await _db.TagBleConfigSnapshots.AddAsync(entity, ct);

    public Task UpdateAsync(TagBleConfigSnapshot entity, CancellationToken ct = default)
    {
        _db.TagBleConfigSnapshots.Update(entity);
        return Task.CompletedTask;
    }

    private IQueryable<TagBleConfigSnapshot> Scoped()
    {
        var tags = RepositoryScope.Tags(_db, _currentUser);
        return _db.TagBleConfigSnapshots.Where(x => tags.Any(t => t.Id == x.TagId));
    }

    public IQueryable<TagBleConfigSnapshot> Query() => _db.TagBleConfigSnapshots.AsQueryable();
}