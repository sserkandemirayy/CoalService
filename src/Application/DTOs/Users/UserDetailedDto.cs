using Application.Common.Extensions;
using Domain.Entities;

namespace Application.DTOs.Users;

public record CompanySummaryDto(
    Guid Id,
    string Name,
    string UserCode);

public record BranchSummaryDto(
    Guid Id,
    string Name,
    Guid CompanyId,
    string CompanyName);

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
    IEnumerable<string>? Permissions = null)
{
    public static UserDetailedDto FromEntity(
        User user,
        bool canViewPII)
    {
        var (phone, address, redacted) =
            MaskingExtensions.ApplyPrivacy(user, canViewPII);

        var companies = user.UserCompanies
            .Where(uc => uc.DeletedAt == null)
            .Select(uc => new CompanySummaryDto(
                uc.Company.Id,
                uc.Company.Name,
                uc.UserCode))
            .ToList();

        var branches = user.UserBranches
            .Where(ub => ub.DeletedAt == null)
            .Select(ub => new BranchSummaryDto(
                ub.Branch.Id,
                ub.Branch.Name,
                ub.Branch.CompanyId,
                ub.Branch.Company.Name))
            .ToList();

        return new UserDetailedDto(
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
            user.UserRoles
                .Where(ur => ur.DeletedAt == null)
                .Select(ur => ur.Role.Name),
            redacted,
            user.UserTypeId,
            user.UserType?.Code,
            user.UserType?.Name,
            user.SpecializationId,
            user.Specialization?.Code,
            user.Specialization?.Name,
            companies,
            branches,
            null);
    }
}

public record UserSpecializationDto(
    Guid Id,
    string Code,
    string Name,
    string? Description,
    Guid UserTypeId,
    string UserTypeName)
{
    public static UserSpecializationDto FromEntity(
        UserSpecialization specialization)
    {
        return new UserSpecializationDto(
            specialization.Id,
            specialization.Code,
            specialization.Name,
            specialization.Description,
            specialization.UserTypeId,
            specialization.UserType.Name ?? string.Empty);
    }
}