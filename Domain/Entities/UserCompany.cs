using Domain.Abstractions;

namespace Domain.Entities;

public class UserCompany : BaseEntity
{
    protected UserCompany() { }

    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!;

    public Guid CompanyId { get; private set; }
    public Company Company { get; private set; } = default!;

    public static UserCompany Create(Guid userId, Guid companyId)
        => new() { Id = Guid.NewGuid(), UserId = userId, CompanyId = companyId };
}