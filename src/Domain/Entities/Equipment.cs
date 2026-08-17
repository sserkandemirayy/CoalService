using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities;

public class Equipment : BaseEntity
{
    protected Equipment()
    {
    }

    public Guid CompanyId { get; private set; }
    public Company Company { get; private set; } = default!;

    public Guid? BranchId { get; private set; }
    public Branch? Branch { get; private set; }

    public Guid CategoryId { get; private set; }
    public EquipmentCategory Category { get; private set; } = default!;

    public string Code { get; private set; } = default!;
    public string Name { get; private set; } = default!;

    public string? SerialNumber { get; private set; }
    public string? Manufacturer { get; private set; }
    public string? Model { get; private set; }

    public EquipmentStatus Status { get; private set; }
        = EquipmentStatus.Active;

    public Guid? FloorMapId { get; private set; }
    public FloorMap? FloorMap { get; private set; }

    public decimal? X { get; private set; }
    public decimal? Y { get; private set; }
    public decimal? Z { get; private set; }

    public DateTime? InstalledAt { get; private set; }
    public DateTime? ExpirationDate { get; private set; }

    public DateTime? LastInspectionAt { get; private set; }
    public DateTime? NextInspectionAt { get; private set; }

    public string? Notes { get; private set; }

    public string? MetadataJson { get; private set; }

    public bool IsActive { get; private set; } = true;

    public ICollection<EquipmentInspection> Inspections { get; private set; }
        = new List<EquipmentInspection>();

    public static Equipment Create(
        Guid companyId,
        Guid? branchId,
        Guid categoryId,
        string code,
        string name,
        string? serialNumber,
        string? manufacturer,
        string? model,
        EquipmentStatus status,
        Guid? floorMapId,
        decimal? x,
        decimal? y,
        decimal? z,
        DateTime? installedAt,
        DateTime? expirationDate,
        DateTime? nextInspectionAt,
        string? notes,
        string? metadataJson)
    {
        if (companyId == Guid.Empty)
            throw new ArgumentException(
                "CompanyId is required.",
                nameof(companyId));

        if (categoryId == Guid.Empty)
            throw new ArgumentException(
                "CategoryId is required.",
                nameof(categoryId));

        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException(
                "Code is required.",
                nameof(code));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Name is required.",
                nameof(name));

        ValidateMapPosition(
            floorMapId,
            x,
            y,
            z);

        return new Equipment
        {
            CompanyId = companyId,
            BranchId = branchId,
            CategoryId = categoryId,

            Code = code.Trim(),
            Name = name.Trim(),

            SerialNumber = serialNumber?.Trim(),
            Manufacturer = manufacturer?.Trim(),
            Model = model?.Trim(),

            Status = status,

            FloorMapId = floorMapId,
            X = x,
            Y = y,
            Z = z,

            InstalledAt = installedAt,
            ExpirationDate = expirationDate,
            NextInspectionAt = nextInspectionAt,

            Notes = notes?.Trim(),
            MetadataJson = metadataJson,

            IsActive = true
        };
    }

    public void Update(
        Guid? branchId,
        Guid categoryId,
        string code,
        string name,
        string? serialNumber,
        string? manufacturer,
        string? model,
        EquipmentStatus status,
        Guid? floorMapId,
        decimal? x,
        decimal? y,
        decimal? z,
        DateTime? installedAt,
        DateTime? expirationDate,
        DateTime? nextInspectionAt,
        string? notes,
        string? metadataJson,
        bool isActive)
    {
        if (categoryId == Guid.Empty)
            throw new ArgumentException(
                "CategoryId is required.",
                nameof(categoryId));

        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException(
                "Code is required.",
                nameof(code));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Name is required.",
                nameof(name));

        ValidateMapPosition(
            floorMapId,
            x,
            y,
            z);

        BranchId = branchId;
        CategoryId = categoryId;

        Code = code.Trim();
        Name = name.Trim();

        SerialNumber = serialNumber?.Trim();
        Manufacturer = manufacturer?.Trim();
        Model = model?.Trim();

        Status = status;

        FloorMapId = floorMapId;
        X = x;
        Y = y;
        Z = z;

        InstalledAt = installedAt;
        ExpirationDate = expirationDate;
        NextInspectionAt = nextInspectionAt;

        Notes = notes?.Trim();
        MetadataJson = metadataJson;

        IsActive = isActive;
    }

    public void RegisterInspection(
        DateTime inspectedAt,
        DateTime? nextInspectionAt)
    {
        LastInspectionAt = inspectedAt;
        NextInspectionAt = nextInspectionAt;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    private static void ValidateMapPosition(
        Guid? floorMapId,
        decimal? x,
        decimal? y,
        decimal? z)
    {
        if (!floorMapId.HasValue)
        {
            if (x.HasValue || y.HasValue || z.HasValue)
            {
                throw new ArgumentException(
                    "FloorMapId is required when map coordinates are supplied.");
            }

            return;
        }

        if (!x.HasValue || !y.HasValue)
        {
            throw new ArgumentException(
                "X and Y are required when FloorMapId is supplied.");
        }
    }
}