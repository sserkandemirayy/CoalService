using Domain.Entities;

namespace Application.DTOs.Users;

// Listeleme için daha hafif versiyon
public record UserSummaryDto(
    Guid Id,
    string Email,
    string FullName,
    string FirstName,
    string LastName,
    bool IsActive,
    DateTime? LastLoginAt,
    Guid? UserTypeId,
    string? UserTypeCode,
    string? UserTypeName
)
{
    public static UserSummaryDto FromEntity(User user)
        => new(
            user.Id,
            user.Email,
            $"{user.FirstName} {user.LastName}".Trim(),
            user.FirstName,
            user.LastName,
            user.IsActive,
            user.LastLoginAt,
            user.UserTypeId,
            user.UserType?.Code,
            user.UserType?.Name
        );
}