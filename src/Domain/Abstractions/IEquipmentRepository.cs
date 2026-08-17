using Domain.Entities;
using Domain.Enums;

namespace Domain.Abstractions;

public interface IEquipmentRepository
{
    Task<Equipment?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default);

    Task<Equipment?> GetByCodeAsync(
        Guid companyId,
        string code,
        CancellationToken ct = default);

    Task<(IReadOnlyList<Equipment> Items, int Total)> GetPagedAsync(
        string? search,
        Guid? companyId,
        Guid? branchId,
        Guid? categoryId,
        Guid? floorMapId,
        EquipmentStatus? status,
        bool? isActive,
        int page,
        int pageSize,
        CancellationToken ct = default);

    Task<IReadOnlyList<Equipment>> GetMapItemsAsync(
        Guid floorMapId,
        CancellationToken ct = default);

    Task AddAsync(
        Equipment equipment,
        CancellationToken ct = default);

    Task UpdateAsync(
        Equipment equipment,
        CancellationToken ct = default);

    IQueryable<Equipment> Query();
}