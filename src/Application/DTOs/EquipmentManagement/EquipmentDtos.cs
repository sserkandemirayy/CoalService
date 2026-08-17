using Domain.Entities;

namespace Application.DTOs.EquipmentManagement;

public sealed record EquipmentCategoryDto(
    Guid Id,
    Guid CompanyId,
    string Code,
    string Name,
    string? Description,
    string? Icon,
    bool ShowOnMap,
    bool IsActive
);

public sealed record EquipmentDto(
    Guid Id,

    Guid CompanyId,
    string? CompanyName,

    Guid? BranchId,
    string? BranchName,

    Guid CategoryId,
    string CategoryCode,
    string CategoryName,
    string? CategoryIcon,
    bool CategoryShowOnMap,

    string Code,
    string Name,

    string? SerialNumber,
    string? Manufacturer,
    string? Model,

    string Status,
    bool IsActive,

    Guid? FloorMapId,
    string? FloorMapName,

    decimal? X,
    decimal? Y,
    decimal? Z,

    DateTime? InstalledAt,
    DateTime? ExpirationDate,

    DateTime? LastInspectionAt,
    DateTime? NextInspectionAt,

    string? Notes,
    string? MetadataJson
);

public sealed record EquipmentInspectionDto(
    Guid Id,
    Guid EquipmentId,

    Guid InspectedByUserId,
    string InspectedByFullName,

    DateTime InspectedAt,
    string Result,

    string? Note,
    DateTime? NextInspectionAt,

    string? DataJson
);

public sealed record EquipmentMapItemDto(
    Guid Id,
    Guid CategoryId,

    string CategoryCode,
    string CategoryName,
    string? Icon,

    string Code,
    string Name,

    string Status,

    decimal X,
    decimal Y,
    decimal? Z
);

public sealed record EquipmentPagedDto(
    IReadOnlyList<EquipmentDto> Items,
    int Total,
    int Page,
    int PageSize
);

public static class EquipmentDtoMapper
{
    public static EquipmentCategoryDto ToDto(
        EquipmentCategory x)
    {
        return new EquipmentCategoryDto(
            x.Id,
            x.CompanyId,
            x.Code,
            x.Name,
            x.Description,
            x.Icon,
            x.ShowOnMap,
            x.IsActive);
    }

    public static EquipmentDto ToDto(
        Equipment x)
    {
        return new EquipmentDto(
            x.Id,

            x.CompanyId,
            x.Company?.Name,

            x.BranchId,
            x.Branch?.Name,

            x.CategoryId,
            x.Category.Code,
            x.Category.Name,
            x.Category.Icon,
            x.Category.ShowOnMap,

            x.Code,
            x.Name,

            x.SerialNumber,
            x.Manufacturer,
            x.Model,

            x.Status.ToString(),
            x.IsActive,

            x.FloorMapId,
            x.FloorMap?.Name,

            x.X,
            x.Y,
            x.Z,

            x.InstalledAt,
            x.ExpirationDate,

            x.LastInspectionAt,
            x.NextInspectionAt,

            x.Notes,
            x.MetadataJson);
    }

    public static EquipmentInspectionDto ToDto(
        EquipmentInspection x)
    {
        var fullName =
            $"{x.InspectedByUser.FirstName} {x.InspectedByUser.LastName}"
                .Trim();

        return new EquipmentInspectionDto(
            x.Id,
            x.EquipmentId,

            x.InspectedByUserId,
            fullName,

            x.InspectedAt,
            x.Result.ToString(),

            x.Note,
            x.NextInspectionAt,

            x.DataJson);
    }
}