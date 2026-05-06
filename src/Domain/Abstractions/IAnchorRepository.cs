using Domain.Entities;
using Domain.Enums;

namespace Domain.Abstractions;

public interface IAnchorRepository
{
    Task<Anchor?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Anchor?> GetByExternalIdAsync(string externalId, CancellationToken ct = default);
    Task<Anchor?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task AddAsync(Anchor anchor, CancellationToken ct = default);
    Task UpdateAsync(Anchor anchor, CancellationToken ct = default);

    Task<(IReadOnlyList<Anchor> Items, int Total)> GetPagedAsync(
        string? search,
        AnchorStatus? status,
        Guid? companyId,
        Guid? branchId,
        int page,
        int pageSize,
        CancellationToken ct = default);

    Task<int> CountAsync(CancellationToken ct = default);
    Task<int> CountByStatusAsync(AnchorStatus status, CancellationToken ct = default);

    IQueryable<Anchor> Query();
}