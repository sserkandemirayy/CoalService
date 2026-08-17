using Application.Common.Models;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.EquipmentManagement.Commands;

public sealed record AddEquipmentInspectionCommand(
    Guid EquipmentId,
    string Result,
    DateTime? InspectedAt,
    string? Note,
    DateTime? NextInspectionAt,
    string? DataJson,
    Guid PerformedByUserId
) : IRequest<Result<Guid>>;

public sealed class AddEquipmentInspectionCommandHandler
    : IRequestHandler<AddEquipmentInspectionCommand, Result<Guid>>
{
    private readonly IEquipmentRepository _equipment;
    private readonly IEquipmentInspectionRepository _inspections;
    private readonly IUnitOfWork _uow;

    public AddEquipmentInspectionCommandHandler(
        IEquipmentRepository equipment,
        IEquipmentInspectionRepository inspections,
        IUnitOfWork uow)
    {
        _equipment = equipment;
        _inspections = inspections;
        _uow = uow;
    }

    public async Task<Result<Guid>> Handle(
        AddEquipmentInspectionCommand request,
        CancellationToken ct)
    {
        var equipment =
            await _equipment.GetByIdAsync(
                request.EquipmentId,
                ct);

        if (equipment is null)
        {
            return Result<Guid>.Failure(
                "Equipment not found.");
        }

        if (!Enum.TryParse<EquipmentInspectionResult>(
                request.Result,
                true,
                out var result))
        {
            return Result<Guid>.Failure(
                "Invalid inspection result.");
        }

        var inspectedAt =
            request.InspectedAt ?? DateTime.UtcNow;

        var inspection =
            EquipmentInspection.Create(
                equipment.Id,
                request.PerformedByUserId,
                inspectedAt,
                result,
                request.Note,
                request.NextInspectionAt,
                request.DataJson);

        inspection.CreatedBy =
            request.PerformedByUserId;

        await _inspections.AddAsync(
            inspection,
            ct);

        equipment.RegisterInspection(
            inspectedAt,
            request.NextInspectionAt);

        equipment.UpdateAudit(
            request.PerformedByUserId);

        await _equipment.UpdateAsync(
            equipment,
            ct);

        await _uow.SaveChangesAsync(ct);

        return Result<Guid>.Success(
            inspection.Id);
    }
}