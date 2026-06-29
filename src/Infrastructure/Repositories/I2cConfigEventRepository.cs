using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class I2cConfigEventRepository : II2cConfigEventRepository
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserService _currentUser;
    public I2cConfigEventRepository(AppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<I2cConfigEvent?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await Scoped().FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task AddAsync(I2cConfigEvent entity, CancellationToken ct = default)
        => await _db.I2cConfigEvents.AddAsync(entity, ct);

    public async Task<(IReadOnlyList<I2cConfigEvent> Items, int Total)> GetPagedByTagIdAsync(
        Guid tagId, int page, int pageSize, CancellationToken ct = default)
    {
        var query = Scoped().Where(x => x.TagId == tagId);
        var total = await query.CountAsync(ct);
        var items = await query.OrderByDescending(x => x.EventTimestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    private IQueryable<I2cConfigEvent> Scoped()
    {
        var tags = RepositoryScope.Tags(_db, _currentUser);
        return _db.I2cConfigEvents.Where(x => tags.Any(t => t.Id == x.TagId));
    }

    public IQueryable<I2cConfigEvent> Query() => _db.I2cConfigEvents.AsQueryable();
}