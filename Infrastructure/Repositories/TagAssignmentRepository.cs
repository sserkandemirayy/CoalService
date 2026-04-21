using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TagAssignmentRepository : ITagAssignmentRepository
{
    private readonly AppDbContext _db;

    public TagAssignmentRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<TagAssignment?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.TagAssignments.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<TagAssignment?> GetActiveByTagIdAsync(Guid tagId, CancellationToken ct = default)
        => await _db.TagAssignments
            .FirstOrDefaultAsync(x => x.TagId == tagId && x.UnassignedAt == null, ct);

    public async Task<TagAssignment?> GetActiveByUserIdAsync(Guid userId, CancellationToken ct = default)
        => await _db.TagAssignments
            .FirstOrDefaultAsync(x => x.UserId == userId && x.UnassignedAt == null, ct);

    public async Task<IEnumerable<TagAssignment>> GetAssignmentsByUserIdAsync(Guid userId, CancellationToken ct = default)
        => await _db.TagAssignments
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.AssignedAt)
            .ToListAsync(ct);

    public async Task<IEnumerable<TagAssignment>> GetAssignmentsByTagIdAsync(Guid tagId, CancellationToken ct = default)
        => await _db.TagAssignments
            .Where(x => x.TagId == tagId)
            .OrderByDescending(x => x.AssignedAt)
            .ToListAsync(ct);

    public async Task AddAsync(TagAssignment assignment, CancellationToken ct = default)
        => await _db.TagAssignments.AddAsync(assignment, ct);

    public Task UpdateAsync(TagAssignment assignment, CancellationToken ct = default)
    {
        _db.TagAssignments.Update(assignment);
        return Task.CompletedTask;
    }

    public IQueryable<TagAssignment> Query() => _db.TagAssignments.AsQueryable();
}