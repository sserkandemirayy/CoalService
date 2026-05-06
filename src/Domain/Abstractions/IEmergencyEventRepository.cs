using Domain.Entities;

namespace Domain.Abstractions;

public interface IEmergencyEventRepository
{
    Task<EmergencyEvent?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(EmergencyEvent emergencyEvent, CancellationToken ct = default);

    Task<(IReadOnlyList<EmergencyEvent> Items, int Total)> GetPagedByTagIdAsync(
        Guid tagId,
        int page,
        int pageSize,
        CancellationToken ct = default);

    IQueryable<EmergencyEvent> Query();
}