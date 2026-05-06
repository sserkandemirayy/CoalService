using Domain.Entities;
using Application.Common.Extensions;

namespace Application.DTOs.Users;

public record CompanySummaryDto(Guid Id, string Name);

public record BranchSummaryDto(Guid Id, string Name, Guid CompanyId, string CompanyName);

public record UserDetailedDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string? Gender,
    DateTime? BirthDate,
    string? Phone,
    string? Address,
    bool IsActive,
    DateTime? LastLoginAt,
    IEnumerable<string> Roles,
    bool PiiRedacted,
    Guid? UserTypeId,
    string? UserTypeCode,
    string? UserTypeName,

    Guid? SpecializationId,
    string? SpecializationCode,
    string? SpecializationName,    
 
    IEnumerable<CompanySummaryDto>? Companies = null,
    IEnumerable<BranchSummaryDto>? Branches = null,
    IEnumerable<string>? Permissions = null
)
{
    public static UserDetailedDto FromEntity(User user, bool canViewPII)
    {
        var (phone, address, redacted) = MaskingExtensions.ApplyPrivacy(user, canViewPII);

        var companies = user.UserCompanies?
            .Select(uc => new CompanySummaryDto(uc.Company.Id, uc.Company.Name))
            .ToList() ?? new List<CompanySummaryDto>();

        var branches = user.UserBranches?
        .Where(ub => ub.DeletedAt == null)
        .Select(ub => new BranchSummaryDto(
            ub.Branch.Id,
            ub.Branch.Name,
            ub.Branch.CompanyId,
            ub.Branch.Company.Name
        ))
        .ToList() ?? new List<BranchSummaryDto>();

        return new(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.Gender,
            user.BirthDate,
            phone,
            address,
            user.IsActive,
            user.LastLoginAt,
            user.UserRoles.Select(ur => ur.Role.Name),
            redacted,
            user.UserTypeId,
            user.UserType?.Code,
            user.UserType?.Name,

            user.SpecializationId,  
            user.Specialization?.Code,
            user.Specialization?.Name,                 
             
            companies,
            branches,
            null            
        );
    }
}

public record UserSpecializationDto(
    Guid Id,
    string Code,
    string Name,
    string? Description,
    Guid UserTypeId,
    string UserTypeName
)
{
    public static UserSpecializationDto FromEntity(UserSpecialization x)
        => new(
            x.Id,
            x.Code,
            x.Name,
            x.Description,
            x.UserTypeId,
            x.UserType.Name ?? ""
        );
}