using Domain.Entities;
using Domain.Enums;

namespace Domain.Abstractions;

public interface ITagRepository
{
    Task<Tag?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Tag?> GetByExternalIdAsync(string externalId, CancellationToken ct = default);
    Task<Tag?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task AddAsync(Tag tag, CancellationToken ct = default);
    Task UpdateAsync(Tag tag, CancellationToken ct = default);

    Task<(IReadOnlyList<Tag> Items, int Total)> GetPagedAsync(
        string? search,
        TagStatus? status,
        TagType? tagType,
        int page,
        int pageSize,
        CancellationToken ct = default);

    Task<int> CountAsync(CancellationToken ct = default);
    Task<int> CountByStatusAsync(TagStatus status, CancellationToken ct = default);

    IQueryable<Tag> Query();
}