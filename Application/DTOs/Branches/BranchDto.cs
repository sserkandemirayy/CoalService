using Domain.Entities;

namespace Application.DTOs.Companies;

public record BranchDto(
    Guid Id,
    Guid CompanyId,
    string Name,
    string Address,
    string Phone,
    string Email,
    string? Code,
    string? City,
    string? District,
    string? PostalCode,
    string? Website,
    Guid? ManagerUserId,
    DateTime? OpenedAt
)
{
    public static BranchDto FromEntity(Branch b) =>
        new(
            b.Id,
            b.CompanyId,
            b.Name,
            b.Address,
            b.Phone,
            b.Email,
            b.Code,
            b.City,
            b.District,
            b.PostalCode,
            b.Website,
            b.ManagerUserId,
            b.OpenedAt
        );
}