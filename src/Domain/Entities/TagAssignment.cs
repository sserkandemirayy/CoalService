using Domain.Abstractions;

namespace Domain.Entities;

public class TagAssignment : BaseEntity
{
    protected TagAssignment() { }

    public Guid TagId { get; private set; }
    public Tag Tag { get; private set; } = default!;

    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!;

    public DateTime AssignedAt { get; private set; }
    public DateTime? UnassignedAt { get; private set; }
    public bool IsPrimary { get; private set; }

    public bool IsActiveAssignment => UnassignedAt == null;

    public static TagAssignment Create(Guid tagId, Guid userId, bool isPrimary = true)
    {
        if (tagId == Guid.Empty)
            throw new ArgumentException("TagId is required.", nameof(tagId));
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId is required.", nameof(userId));

        return new TagAssignment
        {
            TagId = tagId,
            UserId = userId,
            AssignedAt = DateTime.UtcNow,
            IsPrimary = isPrimary
        };
    }

    public void Unassign(DateTime? unassignedAt = null)
    {
        UnassignedAt = unassignedAt ?? DateTime.UtcNow;
        IsPrimary = false;
    }

    public void MarkPrimary() => IsPrimary = true;
}