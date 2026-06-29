using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TagUwbConfigSnapshotRepository : ITagUwbConfigSnapshotRepository
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserService _currentUser;
    public TagUwbConfigSnapshotRepository(AppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<TagUwbConfigSnapshot?> GetByTagIdAsync(Guid tagId, CancellationToken ct = default)
        => await Scoped().FirstOrDefaultAsync(x => x.TagId == tagId, ct);

    public async Task AddAsync(TagUwbConfigSnapshot entity, CancellationToken ct = default)
        => await _db.TagUwbConfigSnapshots.AddAsync(entity, ct);

    public Task UpdateAsync(TagUwbConfigSnapshot entity, CancellationToken ct = default)
    {
        _db.TagUwbConfigSnapshots.Update(entity);
        return Task.CompletedTask;
    }

    private IQueryable<TagUwbConfigSnapshot> Scoped()
    {
        var tags = RepositoryScope.Tags(_db, _currentUser);
        return _db.TagUwbConfigSnapshots.Where(x => tags.Any(t => t.Id == x.TagId));
    }

    public IQueryable<TagUwbConfigSnapshot> Query() => _db.TagUwbConfigSnapshots.AsQueryable();
}