using Domain.Entities;

namespace Application.DTOs.Users;

public record UserDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    bool IsActive,
    DateTime? LastLoginAt,
    IEnumerable<string> Roles,
    Guid? UserTypeId,
    string? UserTypeCode,
    string? UserTypeName,
    Guid? SpecializationId,
    string? SpecializationCode,
    string? SpecializationName       
)
{
    public static UserDto FromEntity(User user)
        => new(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.IsActive,
            user.LastLoginAt,
            user.UserRoles.Select(ur => ur.Role.Name),
            user.UserTypeId,
            user.UserType?.Code,
            user.UserType?.Name,
            user.SpecializationId,   
            user.Specialization?.Code,
            user.Specialization?.Name                  
        );
}