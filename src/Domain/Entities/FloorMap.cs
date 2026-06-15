using Domain.Abstractions;

namespace Domain.Entities;

public class FloorMap : BaseEntity
{
    protected FloorMap() { }

    public string Code { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }

    public Guid? CompanyId { get; private set; }
    public Company? Company { get; private set; }

    public Guid? BranchId { get; private set; }
    public Branch? Branch { get; private set; }

    public decimal Width { get; private set; }
    public decimal Height { get; private set; }
    public string Unit { get; private set; } = "meter";

    public bool IsActive { get; private set; } = true;

    public ICollection<FloorMapFile> Files { get; private set; } = new List<FloorMapFile>();
    public ICollection<FloorMapCalibration> Calibrations { get; private set; } = new List<FloorMapCalibration>();
    public ICollection<AnchorMapPosition> AnchorPositions { get; private set; } = new List<AnchorMapPosition>();
    public ICollection<FloorMapZone> Zones { get; private set; } = new List<FloorMapZone>();

    public static FloorMap Create(
        string code,
        string name,
        string? description,
        Guid? companyId,
        Guid? branchId,
        decimal width,
        decimal height,
        string? unit = "meter")
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code is required.", nameof(code));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));

        return new FloorMap
        {
            Code = code.Trim(),
            Name = name.Trim(),
            Description = description?.Trim(),
            CompanyId = companyId,
            BranchId = branchId,
            Width = width,
            Height = height,
            Unit = string.IsNullOrWhiteSpace(unit) ? "meter" : unit.Trim(),
            IsActive = true
        };
    }

    public void Update(
        string code,
        string name,
        string? description,
        Guid? companyId,
        Guid? branchId,
        decimal width,
        decimal height,
        string? unit)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code is required.", nameof(code));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));

        Code = code.Trim();
        Name = name.Trim();
        Description = description?.Trim();
        CompanyId = companyId;
        BranchId = branchId;
        Width = width;
        Height = height;
        Unit = string.IsNullOrWhiteSpace(unit) ? "meter" : unit.Trim();
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
}