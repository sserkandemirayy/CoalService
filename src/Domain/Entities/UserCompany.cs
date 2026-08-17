using Domain.Abstractions;

namespace Domain.Entities;

public class UserCompany : BaseEntity
{
    public const int MaximumUserCodeLength = 8;

    protected UserCompany()
    {
    }

    public Guid UserId { get; private set; }

    public User User { get; private set; } = default!;

    public Guid CompanyId { get; private set; }

    public Company Company { get; private set; } = default!;

    public string UserCode { get; private set; } = default!;

    public static UserCompany Create(
        Guid userId,
        Guid companyId,
        string userCode)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(
                "UserId is required.",
                nameof(userId));

        if (companyId == Guid.Empty)
            throw new ArgumentException(
                "CompanyId is required.",
                nameof(companyId));

        return new UserCompany
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CompanyId = companyId,
            UserCode = NormalizeUserCode(userCode)
        };
    }

    public void SetUserCode(string userCode)
    {
        UserCode = NormalizeUserCode(userCode);
    }

    public static string NormalizeUserCode(string userCode)
    {
        if (string.IsNullOrWhiteSpace(userCode))
        {
            throw new ArgumentException(
                "UserCode is required.",
                nameof(userCode));
        }

        var normalized = userCode
            .Trim()
            .ToUpperInvariant();

        if (normalized.Length > MaximumUserCodeLength)
        {
            throw new ArgumentException(
                $"UserCode cannot exceed {MaximumUserCodeLength} characters.",
                nameof(userCode));
        }

        if (!normalized.All(x =>
                char.IsLetterOrDigit(x) ||
                x == '-' ||
                x == '_'))
        {
            throw new ArgumentException(
                "UserCode can contain only letters, numbers, '-' and '_'.",
                nameof(userCode));
        }

        return normalized;
    }
}