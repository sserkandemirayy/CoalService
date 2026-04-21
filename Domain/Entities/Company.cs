using Domain.Abstractions;

namespace Domain.Entities;

public class Company : BaseEntity
{
    private Company() { }

    public string Name { get; private set; } = default!;
    public string TaxNumber { get; private set; } = default!;
    public string Address { get; private set; } = default!;
    public string Phone { get; private set; } = default!;
    public string Email { get; private set; } = default!;

    // Yeni alanlar
    public string? Title { get; private set; }           // Ticari Unvan
    public string? TaxOffice { get; private set; }       // Vergi Dairesi
    public string? City { get; private set; }
    public string? District { get; private set; }
    public string? PostalCode { get; private set; }
    public string? Website { get; private set; }
    public string? LogoUrl { get; private set; }         // Logo opsiyonel
    public string? WorkHours { get; private set; }       // JSON ya da düz metin
    public string? Holidays { get; private set; }        // JSON ya da düz metin

    // Navigation
    public ICollection<Branch> Branches { get; private set; } = new List<Branch>();

    public ICollection<UserCompany> UserCompanies { get; private set; } = new List<UserCompany>();

    // === Factory ===
    public static Company Create(
        string name,
        string taxNumber,
        string address,
        string phone,
        string email,
        string? title = null,
        string? taxOffice = null,
        string? city = null,
        string? district = null,
        string? postalCode = null,
        string? website = null,
        string? logoUrl = null,
        string? workHours = null,
        string? holidays = null)
    {
        return new Company
        {
            Name = name,
            TaxNumber = taxNumber,
            Address = address,
            Phone = phone,
            Email = email,
            Title = title,
            TaxOffice = taxOffice,
            City = city,
            District = district,
            PostalCode = postalCode,
            Website = website,
            LogoUrl = logoUrl,
            WorkHours = workHours,
            Holidays = holidays
        };
    }

    // === Update ===
    public void Update(
        string name,
        string address,
        string phone,
        string email,
        string taxNumber,                  
        string? title = null,
        string? taxOffice = null,
        string? city = null,
        string? district = null,
        string? postalCode = null,
        string? website = null,
        string? logoUrl = null,
        string? workHours = null,
        string? holidays = null)
    {
        Name = name;
        Address = address;
        Phone = phone;
        Email = email;
        TaxNumber = taxNumber;             
        Title = title;
        TaxOffice = taxOffice;
        City = city;
        District = district;
        PostalCode = postalCode;
        Website = website;
        LogoUrl = logoUrl;
        WorkHours = workHours;
        Holidays = holidays;
    }
}