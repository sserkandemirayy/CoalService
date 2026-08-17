using Domain.Entities;

namespace Domain.Abstractions;

public interface IEquipmentCategoryRepository
{
    Task<EquipmentCategory?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default);

    Task<EquipmentCategory?> GetByCodeAsync(
        Guid companyId,
        string code,
        CancellationToken ct = default);

    Task<IReadOnlyList<EquipmentCategory>> GetAllAsync(
        Guid? companyId = null,
        bool? isActive = null,
        CancellationToken ct = default);

    Task AddAsync(
        EquipmentCategory category,
        CancellationToken ct = default);

    Task UpdateAsync(
        EquipmentCategory category,
        CancellationToken ct = default);

    IQueryable<EquipmentCategory> Query();
}