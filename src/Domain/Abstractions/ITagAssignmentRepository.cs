using Domain.Entities;

namespace Domain.Abstractions;

public interface ITagAssignmentRepository
{
    Task<TagAssignment?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<TagAssignment?> GetActiveByTagIdAsync(Guid tagId, CancellationToken ct = default);
    Task<TagAssignment?> GetActiveByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<IEnumerable<TagAssignment>> GetAssignmentsByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<IEnumerable<TagAssignment>> GetAssignmentsByTagIdAsync(Guid tagId, CancellationToken ct = default);
    Task AddAsync(TagAssignment assignment, CancellationToken ct = default);
    Task UpdateAsync(TagAssignment assignment, CancellationToken ct = default);
    IQueryable<TagAssignment> Query();
}