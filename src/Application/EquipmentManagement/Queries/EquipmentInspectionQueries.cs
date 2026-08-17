using Application.Common.Models;
using Application.DTOs.EquipmentManagement;
using Domain.Abstractions;
using MediatR;

namespace Application.EquipmentManagement.Queries;

public sealed record GetEquipmentInspectionsQuery(
    Guid EquipmentId
) : IRequest<Result<IReadOnlyList<EquipmentInspectionDto>>>;

public sealed class GetEquipmentInspectionsQueryHandler
    : IRequestHandler<
        GetEquipmentInspectionsQuery,
        Result<IReadOnlyList<EquipmentInspectionDto>>>
{
    private readonly IEquipmentRepository _equipment;
    private readonly IEquipmentInspectionRepository _inspections;

    public GetEquipmentInspectionsQueryHandler(
        IEquipmentRepository equipment,
        IEquipmentInspectionRepository inspections)
    {
        _equipment = equipment;
        _inspections = inspections;
    }

    public async Task<Result<IReadOnlyList<EquipmentInspectionDto>>> Handle(
        GetEquipmentInspectionsQuery request,
        CancellationToken ct)
    {
        var equipment =
            await _equipment.GetByIdAsync(
                request.EquipmentId,
                ct);

        if (equipment is null)
        {
            return Result<IReadOnlyList<EquipmentInspectionDto>>
                .Failure(
                    "Equipment not found.");
        }

        var inspections =
            await _inspections.GetByEquipmentIdAsync(
                request.EquipmentId,
                ct);

        var dto = inspections
            .Select(EquipmentDtoMapper.ToDto)
            .ToList();

        return Result<IReadOnlyList<EquipmentInspectionDto>>
            .Success(dto);
    }
}