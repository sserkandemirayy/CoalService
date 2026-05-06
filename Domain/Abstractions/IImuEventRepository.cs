using Domain.Entities;

namespace Domain.Abstractions;

public interface IImuEventRepository
{
    Task<ImuEvent?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(ImuEvent imuEvent, CancellationToken ct = default);

    Task<(IReadOnlyList<ImuEvent> Items, int Total)> GetPagedByTagIdAsync(
        Guid tagId,
        int page,
        int pageSize,
        CancellationToken ct = default);

    IQueryable<ImuEvent> Query();
}