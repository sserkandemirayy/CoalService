using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TagI2cConfigSnapshotRepository : ITagI2cConfigSnapshotRepository
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserService _currentUser;
    public TagI2cConfigSnapshotRepository(AppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<TagI2cConfigSnapshot?> GetByTagIdAsync(Guid tagId, CancellationToken ct = default)
        => await Scoped().FirstOrDefaultAsync(x => x.TagId == tagId, ct);

    public async Task AddAsync(TagI2cConfigSnapshot entity, CancellationToken ct = default)
        => await _db.TagI2cConfigSnapshots.AddAsync(entity, ct);

    public Task UpdateAsync(TagI2cConfigSnapshot entity, CancellationToken ct = default)
    {
        _db.TagI2cConfigSnapshots.Update(entity);
        return Task.CompletedTask;
    }

    private IQueryable<TagI2cConfigSnapshot> Scoped()
    {
        var tags = RepositoryScope.Tags(_db, _currentUser);
        return _db.TagI2cConfigSnapshots.Where(x => tags.Any(t => t.Id == x.TagId));
    }

    public IQueryable<TagI2cConfigSnapshot> Query() => _db.TagI2cConfigSnapshots.AsQueryable();
}