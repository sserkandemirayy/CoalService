using Domain.Entities;

namespace Domain.Abstractions;

public interface IEquipmentInspectionRepository
{
    Task<EquipmentInspection?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default);

    Task<IReadOnlyList<EquipmentInspection>> GetByEquipmentIdAsync(
        Guid equipmentId,
        CancellationToken ct = default);

    Task AddAsync(
        EquipmentInspection inspection,
        CancellationToken ct = default);

    IQueryable<EquipmentInspection> Query();
}