using Domain.Abstractions;

namespace Domain.Entities;

public class UserCompany : BaseEntity
{
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
            throw new ArgumentException("UserId is required.", nameof(userId));

        if (companyId == Guid.Empty)
            throw new ArgumentException("CompanyId is required.", nameof(companyId));

        if (string.IsNullOrWhiteSpace(userCode))
            throw new ArgumentException("UserCode is required.", nameof(userCode));

        return new UserCompany
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CompanyId = companyId,
            UserCode = userCode
        };
    }

    public void SetUserCode(string userCode)
    {
        if (string.IsNullOrWhiteSpace(userCode))
            throw new ArgumentException("UserCode is required.", nameof(userCode));

        UserCode = userCode;
    }
}