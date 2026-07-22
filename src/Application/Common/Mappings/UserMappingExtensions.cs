using Domain.Entities;

namespace Application.Common.Mappings;

public static class UserMappingExtensions
{
    public static string? GetFullName(this User? user)
    {
        if (user is null)
            return null;

        var fullName = string.Join(
            " ",
            new[]
            {
                user.FirstName?.Trim(),
                user.LastName?.Trim()
            }
            .Where(x => !string.IsNullOrWhiteSpace(x)));

        return string.IsNullOrWhiteSpace(fullName)
            ? null
            : fullName;
    }
}