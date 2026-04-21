using Domain.Abstractions;
using Domain.Entities;

public class UserBranch : BaseEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!;

    public Guid BranchId { get; private set; }
    public Branch Branch { get; private set; } = default!;

    protected UserBranch() { }

    public static UserBranch Create(Guid userId, Guid branchId, Guid? createdBy = null)
        => new()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            BranchId = branchId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
}
