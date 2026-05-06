using Domain.Entities;

namespace Application.DTOs.Companies;

public record CompanyDto(
    Guid Id,
    string Name,
    string TaxNumber,
    string Address,
    string Phone,
    string Email,
    string? Title,
    string? TaxOffice,
    string? City,
    string? District,
    string? PostalCode,
    string? Website,
    string? LogoUrl,
    string? WorkHours,
    string? Holidays,
    int BranchCount
)
{
    public static CompanyDto FromEntity(Company c) =>
        new(
            c.Id,
            c.Name,
            c.TaxNumber,
            c.Address,
            c.Phone,
            c.Email,
            c.Title,
            c.TaxOffice,
            c.City,
            c.District,
            c.PostalCode,
            c.Website,
            c.LogoUrl,
            c.WorkHours,
            c.Holidays,
            c.Branches?.Count ?? 0
        );
}