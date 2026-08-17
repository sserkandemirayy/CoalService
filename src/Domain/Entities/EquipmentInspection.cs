using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities;

public class EquipmentInspection : BaseEntity
{
    protected EquipmentInspection()
    {
    }

    public Guid EquipmentId { get; private set; }
    public Equipment Equipment { get; private set; } = default!;

    public Guid InspectedByUserId { get; private set; }
    public User InspectedByUser { get; private set; } = default!;

    public DateTime InspectedAt { get; private set; }

    public EquipmentInspectionResult Result { get; private set; }

    public string? Note { get; private set; }

    public DateTime? NextInspectionAt { get; private set; }

    public string? DataJson { get; private set; }

    public static EquipmentInspection Create(
        Guid equipmentId,
        Guid inspectedByUserId,
        DateTime inspectedAt,
        EquipmentInspectionResult result,
        string? note,
        DateTime? nextInspectionAt,
        string? dataJson)
    {
        if (equipmentId == Guid.Empty)
            throw new ArgumentException(
                "EquipmentId is required.",
                nameof(equipmentId));

        if (inspectedByUserId == Guid.Empty)
            throw new ArgumentException(
                "InspectedByUserId is required.",
                nameof(inspectedByUserId));

        return new EquipmentInspection
        {
            EquipmentId = equipmentId,
            InspectedByUserId = inspectedByUserId,
            InspectedAt = inspectedAt,
            Result = result,
            Note = note?.Trim(),
            NextInspectionAt = nextInspectionAt,
            DataJson = dataJson
        };
    }
}