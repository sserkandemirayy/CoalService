using Domain.Abstractions;

namespace Domain.Entities;

public class EquipmentCategory : BaseEntity
{
    protected EquipmentCategory()
    {
    }

    public Guid CompanyId { get; private set; }
    public Company Company { get; private set; } = default!;

    public string Code { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }

    public string? Icon { get; private set; }

    public bool ShowOnMap { get; private set; } = true;
    public bool IsActive { get; private set; } = true;

    public ICollection<Equipment> Equipments { get; private set; }
        = new List<Equipment>();

    public static EquipmentCategory Create(
        Guid companyId,
        string code,
        string name,
        string? description,
        string? icon,
        bool showOnMap)
    {
        if (companyId == Guid.Empty)
            throw new ArgumentException(
                "CompanyId is required.",
                nameof(companyId));

        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException(
                "Code is required.",
                nameof(code));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Name is required.",
                nameof(name));

        return new EquipmentCategory
        {
            CompanyId = companyId,
            Code = code.Trim(),
            Name = name.Trim(),
            Description = description?.Trim(),
            Icon = icon?.Trim(),
            ShowOnMap = showOnMap,
            IsActive = true
        };
    }

    public void Update(
        string code,
        string name,
        string? description,
        string? icon,
        bool showOnMap,
        bool isActive)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException(
                "Code is required.",
                nameof(code));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Name is required.",
                nameof(name));

        Code = code.Trim();
        Name = name.Trim();
        Description = description?.Trim();
        Icon = icon?.Trim();
        ShowOnMap = showOnMap;
        IsActive = isActive;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}