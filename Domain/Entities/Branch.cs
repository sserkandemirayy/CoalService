using Domain.Abstractions;

namespace Domain.Entities;

public class Branch : BaseEntity
{
    private Branch() { }

    public Guid CompanyId { get; private set; }
    public string Name { get; private set; } = default!;
    public string Address { get; private set; } = default!;
    public string Phone { get; private set; } = default!;
    public string Email { get; private set; } = default!;

    // Yeni Alanlar
    public string? Code { get; private set; }
    public string? City { get; private set; }
    public string? District { get; private set; }
    public string? PostalCode { get; private set; }
    public string? Website { get; private set; }
    public Guid? ManagerUserId { get; private set; }
    public DateTime? OpenedAt { get; private set; }

    // Navigation
    public Company Company { get; private set; } = default!;

    public ICollection<UserBranch> UserBranches { get; private set; } = new List<UserBranch>();

    public static Branch Create(
        Guid companyId,
        string name,
        string address,
        string phone,
        string email,
        string? code = null,
        string? city = null,
        string? district = null,
        string? postalCode = null,
        string? website = null,
        Guid? managerUserId = null,
        DateTime? openedAt = null
    )
    {
        return new Branch
        {
            CompanyId = companyId,
            Name = name,
            Address = address,
            Phone = phone,
            Email = email,
            Code = code,
            City = city,
            District = district,
            PostalCode = postalCode,
            Website = website,
            ManagerUserId = managerUserId,
            OpenedAt = openedAt
        };
    }

    public void Update(
        string name,
        string address,
        string phone,
        string email,
        string? code = null,
        string? city = null,
        string? district = null,
        string? postalCode = null,
        string? website = null,
        Guid? managerUserId = null,
        DateTime? openedAt = null
    )
    {
        Name = name;
        Address = address;
        Phone = phone;
        Email = email;
        Code = code;
        City = city;
        District = district;
        PostalCode = postalCode;
        Website = website;
        ManagerUserId = managerUserId;
        OpenedAt = openedAt;
    }
}