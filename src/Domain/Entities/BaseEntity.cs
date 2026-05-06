namespace Domain.Abstractions;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }

    public bool IsDeleted => DeletedAt != null;

    public void SoftDelete(Guid userId)
    {
        DeletedAt = DateTime.UtcNow;
        DeletedBy = userId;
    }

    public void UpdateAudit(Guid userId)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = userId;
    }
}
