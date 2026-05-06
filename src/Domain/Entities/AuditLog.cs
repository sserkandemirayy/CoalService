using System.Text.Json;

namespace Domain.Entities;

public class AuditLog
{
    private AuditLog() { }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid? UserId { get; private set; }
    public string Action { get; private set; } = default!;
    public string ResourceType { get; private set; } = default!;
    public Guid? ResourceId { get; private set; }
    public string? IpAddress { get; private set; }

    // JSONB kolon
    public JsonDocument? Detail { get; private set; }
    public DateTime Timestamp { get; private set; } = DateTime.UtcNow;

    public static AuditLog Create(Guid? userId, string action, string resourceType, Guid? resourceId, string? ip, object? detailObj)
    {
        return new AuditLog
        {
            UserId = userId,
            Action = action,
            ResourceType = resourceType,
            ResourceId = resourceId,
            IpAddress = ip,
            Detail = detailObj != null
                ? JsonDocument.Parse(JsonSerializer.Serialize(detailObj))
                : null
        };
    }
}
