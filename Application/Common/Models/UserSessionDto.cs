using Domain.Entities;

public record UserSessionDto(Guid Id, string? Device, string? Ip, string? UserAgent,
    DateTime CreatedAt, DateTime? LastSeenAt, DateTime ExpiresAt)
{
    public static UserSessionDto FromEntity(RefreshToken t)
        => new(t.Id, t.Device, t.IpAddress, t.UserAgent, t.CreatedAt, t.LastUsedAt, t.ExpiresAt);
}
